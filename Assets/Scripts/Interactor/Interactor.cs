using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Interactor : NetworkBehaviour
{
    [SerializeField] private float _MiningDuration = 1.0f;
    [SerializeField] private Item _ItemToSpawn;
    [HideInInspector] public Tile _OwnerTile;
    private EMaterialType _MaterialType;
    public EMaterialType MaterialType { get => _MaterialType; set => _MaterialType = value; }

    IEnumerator _MiningEnumHandler;
    Player_Mining _Player;
    Renderer _MeshRenderer;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => IsSpawned);
        if (IsServer)
        {
            _OwnerTile.ChangeTilePosition += UpdateInteractorPosition;
            _MeshRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void OnDisable()
    {
        if(IsServer) 
            _OwnerTile.ChangeTilePosition -= UpdateInteractorPosition;
        if(_Player != null)
        {
            _Player.OnStopMining();
            _Player = null;
        }
    }

    public void StartMining(Player_Mining player)
    {
        _Player = player;
        if (_MiningEnumHandler != null)
            StopCoroutine(_MiningEnumHandler);
        _MiningEnumHandler = MiningEnum();
        StartCoroutine(_MiningEnumHandler);
    }

    public void StopMining()
    {
        if (_MiningEnumHandler == null) return;
        StopCoroutine(_MiningEnumHandler);
        _Player.OnStopMining();
        _Player = null;
    }

    private IEnumerator MiningEnum()
    {
        yield return new WaitForSeconds(_MiningDuration);
        _Player.OnStopMining();
        _Player = null;

        OnInteractorMinedServerRpc();
    }


    [ServerRpc(RequireOwnership =false)]
    public void OnInteractorMinedServerRpc()
    {
        NetworkObject ntwObject = Instantiate(_ItemToSpawn, transform.position + Vector3.up * 10, Quaternion.identity).GetComponent<NetworkObject>();
        ntwObject.Spawn(true);
        OnInteractorMinedClientRpc();
    }

    [ClientRpc]
    public void OnInteractorMinedClientRpc()
    {
        if(IsOwner) 
        gameObject.SetActive(false);
    }

    private void UpdateInteractorPosition(float positionY)
    {
        UpdatePositionClientRpc(new Vector3(transform.position.x, positionY, transform.position.z));
    }

    [ClientRpc]
    private void UpdatePositionClientRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
