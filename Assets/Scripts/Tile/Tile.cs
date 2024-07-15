using System.Collections;
using UnityEngine;
using TriInspector;
using Unity.Netcode;
using System;

[DeclareBoxGroup("Spawn Interactors")]
public class Tile : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<ETileTypes> _TileType = new NetworkVariable<ETileTypes>();
    [SerializeField, Group("Spawn Interactors")] private ESpawnPositions _SpawnPositions = ESpawnPositions.Nothing;
    [SerializeField, Group("Spawn Interactors")] private EMaterialType _SpawnMaterialType = EMaterialType.Neutral;
 
    [SerializeField] private NetworkVariable<bool> _Walkable = new NetworkVariable<bool>();
    private Vector2Int _Coordinates;

    private NetworkVariable<float> _Height = new NetworkVariable<float>();
    private IEnumerator _DegradationEnumHandle;
    private TileEditorFunctions _TileEditorFunctions;

    public delegate void OnNetworkConnect();
    public OnNetworkConnect _onNetworkConnect;

    public Action<float> ChangeTilePosition;

    #region public fields
    public Vector2Int Coordinates { get => _Coordinates; set => _Coordinates = value; }

    private Vector2Int[] _AdjacentTilesCoordinates;
    public Vector2Int[] AdjacentTilesCoordinates { get => _AdjacentTilesCoordinates;}
    public bool Walkable { get => _Walkable.Value; set => _Walkable.Value = value; }
    public ETileTypes TileType { get => _TileType.Value; set => _TileType.Value = value; }
    public ESpawnPositions SpawnPositions { get => _SpawnPositions; set => _SpawnPositions = value; }
    public float Height { get => _Height.Value; set => _Height.Value = value; }
    #endregion

    private void OnEnable()
    {
        MonoBehaviourExtension._onSessionStarted += OnSessionStarted;
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        MonoBehaviourExtension._onSessionStarted -= OnSessionStarted;
        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    private void Start()
    {
        _TileEditorFunctions = GetComponent<TileEditorFunctions>();
        _TileEditorFunctions.UpdateModel();
    }

    public void OnSessionStarted()
    {
        if (IsServer)
        {
            _DegradationEnumHandle = DegradationTimer();
            StartCoroutine(_DegradationEnumHandle);
        }

    }

    public void OnClientConnected(ulong clientId)
    {

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        _onNetworkConnect?.Invoke();
        base.OnNetworkSpawn();
    }


    public void UpdateClientsPosition(float numberOfTilesToFall = 1)
    {
        StartCoroutine(MoveTileCoroutine(numberOfTilesToFall));
    }

    IEnumerator MoveTileCoroutine(float numberOfTilesToFall)
    {
        float i = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position - Vector3.up * numberOfTilesToFall;
        while (i < 1)
        {
            i += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, i);
            ChangeTilePosition?.Invoke(transform.position.y);
            yield return null;
        }
        transform.position = endPosition;
        ChangeTilePosition?.Invoke(transform.position.y);
    }

    IEnumerator DegradationTimer()
    {
        while(transform.position.y >= -1)
        {
            float timerValue = GetDegradationTimer();
            yield return new WaitUntil(() => TileType != ETileTypes.Sand && TileType != ETileTypes.Undegradable);
            yield return new WaitForSeconds(timerValue);
            UpdateClientsPosition();
        }
        UpdateClientsPosition(10);
    }

    private float GetDegradationTimer()
    {
        float randomTimeValue = UnityEngine.Random.Range(_TileEditorFunctions.TileGeneralData.baseMinTimerDegradation, _TileEditorFunctions.TileGeneralData.baseMaxTimerDegradation);
        return randomTimeValue / DegradationManager.Instance.GetDegradationEvolutionValue() / DegradationManager.Instance.GetDegradationMultByTileType(TileType);
    }


    IEnumerator UpdateData(bool walkable, ETileTypes tileType, float height)
    {
        yield return new WaitUntil(() => MonoBehaviourExtension.Instance.IsSessionStarted);
        UpdateDataRpc(walkable, tileType, height);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateDataRpc( bool walkable, ETileTypes tileType, float height)
    {
        int i = (int)transform.position.x;
        int j = (int)transform.position.z;
        Coordinates = new Vector2Int(i, j);
        gameObject.name = i + "  " + j;
        Walkable = walkable;
        TileType = tileType;
        Height = height;
        UpdateModel();

    }

    public void UpdateModel() => GetComponent<TileEditorFunctions>().UpdateModel();

#if UNITY_EDITOR
    void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

    private void _OnValidate()
    {
        if (this == null) return;
        GetComponent<TileEditorFunctions>().UpdateModel();
    }

    [Button, Group("Spawn Interactors")]
    public void SpawnItems()
    {
        GetComponent<TileEditorFunctions>().SpawnInteractors(_SpawnMaterialType, _SpawnPositions);
    }
#endif

}

