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
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        _onNetworkConnect?.Invoke();
        base.OnNetworkSpawn();
    }

    [ClientRpc]
    public void UpdateClientsPositionClientRpc(float numberOfTilesToFall = 1)
    {
        transform.DOMoveY(transform.position.y - numberOfTilesToFall, 2).SetEase(Ease.InOutExpo);
    }
}
