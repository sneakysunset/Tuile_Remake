using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GridUtils
{
    public static Tile WorldPosToTile(Vector3 pos)
    {
        TileSystem ts;
        if (TileSystem.Instance != null) ts = TileSystem.Instance;
        else ts = GameObject.FindObjectOfType<TileSystem>();
        float xOffset = 0;
        int x;
        int z;


        z = Mathf.RoundToInt(pos.z / (ts.TilePrefab.transform.localScale.z * 1.48f));
        if (z % 2 == 1) xOffset = ts.TilePrefab.transform.localScale.x * .9f;
        x = Mathf.RoundToInt((pos.x - xOffset) / (ts.TilePrefab.transform.localScale.x * 1.7f));
        if (ts.Tiles != null && ts.Tiles.GetLength(0) > x && ts.Tiles.GetLength(1) > z && 0 <= x && 0 <= z) return ts.Tiles[x, z];
        else return null;
    }

    public static Vector3 indexToWorldPos(int x, int z, Transform tT)
    {
        float xOffset = 0;
        if (z % 2 == 1) xOffset = tT.localScale.x * .85f;
        Vector3 pos = new Vector3(x * tT.localScale.x * 1.7f + xOffset, 0, z * tT.localScale.z * 1.48f);

        return pos;
    }

    public static bool TContain(Tile[,] Tiles, Tile tile)
    {
        for (int i = 0; i < Tiles.GetLength(0); i++)
        {
            for(int j = 0; j < Tiles.GetLength(1); j++)
            {
                if (Tiles[i, j] == tile) return true;
            }
        }
        return false;
    }


    public static List<Tile> GetTilesAround(int numOfRows, Tile tile)
    {
        TileSystem tis = TileSystem.Instance;
        Dictionary<Tile, bool> tileDict = new Dictionary<Tile, bool>();
        int rowsSeen = 0;
        tileDict.Add(tile, false);
        while (rowsSeen < numOfRows)
        {
            int ix = tileDict.Count;
            foreach(var pair in  tileDict)
            {
                if (!pair.Value)
                {
                    foreach (Vector2Int vecs in pair.Key.AdjacentTilesCoordinates)
                    {
                        if (vecs.x >= 0 && vecs.x < tis.TileRows && vecs.y >= 0 && vecs.y < tis.TileColumns && tis.Tiles[vecs.x, vecs.y].Walkable && !tileDict.Any(m => m.Key == tis.Tiles[vecs.x, vecs.y]))
                        {
                            tileDict.Add(tis.Tiles[vecs.x, vecs.y], false);
                        }
                    }
                    tileDict[pair.Key] = true;
                }
            }
            rowsSeen++;
        }

        List<Tile> result = new List<Tile>();
        foreach (Tile t in tileDict.Keys)
        {
            result.Add(t);
        }
        return  result;
    }


/*    public static List<Tile> GetTilesBetweenRaws(int rowMin, int rowMax, Tile tile)
    {
        TileSystem tis = TileSystem.Instance;
        List<Tile> ts = new List<Tile>();
        List<Tile> ts2 = new List<Tile>();
        int rowsSeen = 0;
        ts.Add(tile);
        while (rowsSeen <= rowMax)
        {
            int ix = ts.Count;
            for (int i = 0; i < ix; i++)
            {
                if (!ts[i].isPathChecked)
                {
                    foreach (Vector2Int vecs in ts[i].adjTCoords)
                    {
                        if (vecs.x >= 0 && vecs.x < tis.rows && vecs.y >= 0 && vecs.y < tis.columns && !ts.Contains(tis.tiles[vecs.x, vecs.y]))
                        {
                            ts.Add(tis.tiles[vecs.x, vecs.y]);
                            if (rowsSeen >= rowMin && rowsSeen <= rowMax)
                            {
                                ts2.Add(tis.tiles[vecs.x, vecs.y]);
                            }
                        }
                    }
                    ts[i].isPathChecked = true;
                }
            }
            rowsSeen++;
        }

        foreach (Tile t in ts)
        {
            t.isPathChecked = false;
        }
        return ts2;
    }*/

}
