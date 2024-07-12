using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TileNetworkOperations : NetworkBehaviour
{
    Tile _Tile;
    public delegate void OnNetworkConnect();
    public OnNetworkConnect _onNetworkConnect;
    public Action<float> ChangeTilePosition;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        _onNetworkConnect?.Invoke();
        base.OnNetworkSpawn();
    }

    [ClientRpc]
    public void UpdateClientsPositionClientRpc(float numberOfTilesToFall = 1)
    {
        StartCoroutine(MoveTileCoroutine(numberOfTilesToFall));
    }

    IEnumerator MoveTileCoroutine(float numberOfTilesToFall)
    {
        float i = 0;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position - Vector3.up * numberOfTilesToFall;
        while(i < 1)
        {
            i += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition,endPosition, i);
            ChangeTilePosition?.Invoke(transform.position.y);
            yield return null;
        }
        transform.position = endPosition;
        ChangeTilePosition?.Invoke(transform.position.y);
    }
}
