using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Tile;

[Serializable]
public class TileData
{
    [XmlElement("Coordinates")]
    public Vector2Int Coordinates;
    [XmlElement("Walkable")]
    public bool Walkable;
    [XmlElement("TileType")]
    public ETileTypes TileType;
    [XmlElement("SpawnPositions")]
    public ESpawnPositions SpawnPositions;
    [XmlElement("Height")]
    public float Height;

    public TileData (Vector2Int coordinates, bool walkable, ETileTypes tileType, ESpawnPositions spawnPositions, float height)
    {
        Coordinates = coordinates;
        Walkable = walkable;
        TileType = tileType;
        SpawnPositions = spawnPositions;
        Height = height;
    }

    public TileData() { }
}

[Serializable]
[XmlRoot("TileDataList")]
public class TileDataWrapper
{
    [XmlArray("Tiles")]
    [XmlArrayItem("TileData")]
    public List<TileData> items;

    public TileDataWrapper() { }

    public TileDataWrapper(List<TileData> items)
    {
        this.items = items;
    }
}
