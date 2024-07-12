using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tile_InteractorSpawner : MonoBehaviour
{
    [HideInInspector] public Interactor _InteractorToSpawn;
    [HideInInspector] public Tile _ParentTile;

    IEnumerator Start()
    {
        yield return new WaitUntil(()=>NetworkManager.Singleton.IsListening);
        if(NetworkManager.Singleton.IsServer)
            SpawnServerRpc();
        GetComponent<MeshRenderer>().enabled = false;
    }

    [ServerRpc]
    void SpawnServerRpc()
    {
        Interactor interactor = Instantiate(_InteractorToSpawn, transform.position, Quaternion.identity);
        interactor.NetworkObject.Spawn();
        SetParentTileClientRpc(interactor);
        interactor._OwnerTile = _ParentTile;
    }

    [ClientRpc]
    void SetParentTileClientRpc(Interactor interactor)
    {
        interactor._OwnerTile = _ParentTile;
    }
}