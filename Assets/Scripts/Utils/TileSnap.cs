using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileSnap : MonoBehaviour
{
    Tile _Tile;

    void Update()
    {
        if (_Tile == null) _Tile = GetComponent<Tile>();
        transform.position = new Vector3(transform.position.x, Mathf.FloorToInt(transform.position.y), transform.position.z);
    }
}
