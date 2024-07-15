using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Unity.Netcode;
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
    [XmlArrayItem("Interactors")]
    public List<string> Interactors;
    [XmlElement("Height")]
    public float Height;

    public TileData() { }
    public TileData (Vector2Int coordinates, bool walkable, ETileTypes tileType, Interactor[] interactors, float height)
    {
        Coordinates = coordinates;
        Walkable = walkable;
        TileType = tileType;
        Interactors = new List<string>();
        foreach(var interactor in interactors)
        {
            string interactorString = $"{interactor.MaterialType}:{interactor.transform.parent.GetSiblingIndex()}";
            Interactors.Add(interactorString);
        }
        Height = height;
    }

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
