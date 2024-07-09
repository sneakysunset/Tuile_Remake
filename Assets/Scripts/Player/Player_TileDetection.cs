using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_TileDetection : MonoBehaviour
{
    private Player _Player;
    Tile _PreviousTile;
    private void Start()
    {
        _Player = GetComponent<Player>();
    }

    private void Update()
    {
        GetTileUnder();
    }

    private void GetTileUnder()
    {
        _Player.TileUnder = GridUtils.WorldPosToTile(transform.position);
        if (_Player.TileUnder != null) OnTileStay(_Player.TileUnder);
        if (_Player.TileUnder != null && _PreviousTile != null && _PreviousTile != _Player.TileUnder)
        {
            OnTileExit(_PreviousTile);
            OnTileEnter(_Player.TileUnder);
        }
        _PreviousTile = _Player.TileUnder;  
    }

    private void OnTileEnter(Tile tile)
    {
        _Player.GroundType = GetTileType(tile.TileType);
    }

    private void OnTileExit(Tile tile)
    {
    }

    private void OnTileStay(Tile tile)
    {
    }

    private string GetTileType(ETileTypes tileUnderType)
    {
        string groundTypeName = "Neutral";
        switch (_Player.TileUnder.TileType)
        {
            case ETileTypes.Wood: groundTypeName = _Player.TileUnder.TileType.ToString(); break;
            case ETileTypes.Rock: groundTypeName = _Player.TileUnder.TileType.ToString(); break;
            case ETileTypes.Sand: groundTypeName = _Player.TileUnder.TileType.ToString(); break;
            default: break;
        }
        return groundTypeName;
    }
}
