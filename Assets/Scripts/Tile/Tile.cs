using System.Collections;
using UnityEngine;
using TriInspector;

[DeclareBoxGroup("Spawn Interactors")]
public class Tile : MonoBehaviour
{
    [SerializeField] private ETileTypes _TileType = ETileTypes.Neutral;
    [SerializeField, Group("Spawn Interactors")] private ESpawnPositions _SpawnPositions = ESpawnPositions.Nothing;
    [SerializeField, Group("Spawn Interactors")] private EMaterialType _SpawnMaterialType = EMaterialType.Neutral;
 
    [SerializeField] private bool _Walkable = false;
    private Vector2Int _Coordinates;

    private float _Height = 0;
    private IEnumerator _DegradationEnumHandle;
    private TileNetworkOperations _TileNetworkOperations;
    private TileEditorFunctions _TileEditorFunctions;
    

    #region public fields
    public Vector2Int Coordinates { get => _Coordinates; set => _Coordinates = value; }

    private Vector2Int[] _AdjacentTilesCoordinates;
    public Vector2Int[] AdjacentTilesCoordinates { get => _AdjacentTilesCoordinates;}
    public bool Walkable { get => _Walkable; set => _Walkable = value; }
    public ETileTypes TileType { get => _TileType; set => _TileType = value; }
    public ESpawnPositions SpawnPositions { get => _SpawnPositions; set => _SpawnPositions = value; }
    public float Height { get => _Height; set => _Height = value; }
    #endregion

    private void OnEnable()
    {
        _TileNetworkOperations = GetComponent<TileNetworkOperations>();
        _TileNetworkOperations._onNetworkConnect += OnNetworkConnection;
    }

    private void OnDisable()
    {
        _TileNetworkOperations._onNetworkConnect -= OnNetworkConnection;
    }

    public void OnNetworkConnection()
    {
        _DegradationEnumHandle = DegradationTimer();
        _TileEditorFunctions = GetComponent<TileEditorFunctions>();   
        _TileNetworkOperations = GetComponent<TileNetworkOperations>(); 

        StartCoroutine(_DegradationEnumHandle);
    }


    IEnumerator DegradationTimer()
    {
        while(transform.position.y >= -1)
        {
            float timerValue = GetDegradationTimer();
            yield return new WaitUntil(() => _TileType != ETileTypes.Sand && _TileType != ETileTypes.Undegradable);
            yield return new WaitForSeconds(timerValue);
            _TileNetworkOperations.UpdateClientsPositionClientRpc();
        }
        _TileNetworkOperations.UpdateClientsPositionClientRpc(10);
    }

    private float GetDegradationTimer()
    {
        float randomTimeValue = Random.Range(_TileEditorFunctions.TileGeneralData.baseMinTimerDegradation, _TileEditorFunctions.TileGeneralData.baseMaxTimerDegradation);
        return randomTimeValue / DegradationManager.Instance.GetDegradationEvolutionValue() / DegradationManager.Instance.GetDegradationMultByTileType(TileType);
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

