using UnityEngine;
using TriInspector;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System;
using System.Xml.Serialization;
using System.Collections;
using Unity.Netcode;

[DeclareTabGroup("Tile Generation"), DeclareHorizontalGroup("Tile Generation/Instantiation")]
public class TileSystem : Singleton<TileSystem>
{
    [SerializeField] private Tile[,] _Tiles;
    public Tile[,] Tiles { get => _Tiles; }
    public int TileRows { get => _TileRows; }
    public int TileColumns { get => _TileColumns; }
    public Tile TilePrefab { get => _TilePrefab; }
    public bool IsGenerationOver { get => _IsGenerationOver; }

    [SerializeField, Group("Tile Generation/Instantiation"), Tab("Tile Generation")] private Tile _TilePrefab;
    [SerializeField, Group("Tile Generation/Instantiation"), Tab("Tile Generation")] private int _TileRows;
    [SerializeField, Group("Tile Generation/Instantiation"), Tab("Tile Generation")] private int _TileColumns;
    [SerializeField, Group("Tile Generation"), Tab("Tile Generation")] private Transform _TileFolder;
    [SerializeField, Group("Tile Generation"), Tab("Tile Generation")] private bool _AllowGridEdition;
    [SerializeField, Group("Tile Generation"), Tab("Data Management")] private string _FileName;
    private bool _IsGenerationOver;
    private List<TileData> _TileDataArray;
    [Button, Group("Tile Generation"), Tab("Data Management")]
    public void SaveArray()
    {
        _TileDataArray = new List<TileData>();
        string filePath = Application.streamingAssetsPath + "\\Maps\\" + _FileName + ".xml";
        foreach (var tile in _Tiles)
        {
            Interactor[] interactors = tile.GetComponentsInChildren<Interactor>();  
            TileData data = new TileData(tile.Coordinates, tile.Walkable, tile.TileType, interactors, tile.transform.position.y);
            _TileDataArray.Add(data);
        }
        TileDataWrapper wrapper = new TileDataWrapper(_TileDataArray);

        XmlSerializer serializer = new XmlSerializer(typeof(TileDataWrapper));
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, wrapper);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(()=> NetworkManager.Singleton.IsListening );
        GetTilesReferences();
        //LoadArray();
    }

#if UNITY_EDITOR
    [Button, Group("Tile Generation"), Tab("Data Management")]
    public void LoadArray() => StartCoroutine(LoadArrayEnum());
    private IEnumerator LoadArrayEnum()
    {
        string filePath = Application.streamingAssetsPath + "\\Maps\\" + _FileName + ".xml";
        if (File.Exists(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TileDataWrapper));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                TileDataWrapper wrapper = (TileDataWrapper)serializer.Deserialize(stream);
                _TileDataArray = wrapper.items;
            }

            int maxRow = _TileDataArray.Max(m => m.Coordinates.x) + 1;
            int maxColumn = _TileDataArray.Max(m => m.Coordinates.y) + 1;
            _Tiles = new Tile[maxRow, maxColumn];
            _TileRows = maxRow;
            _TileColumns = maxColumn;

            DestroyGrid();

            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxColumn; j++)
                {
                    TileData tileD = _TileDataArray.Where(m => m.Coordinates.x == i && m.Coordinates.y == j).FirstOrDefault();

                    _Tiles[i, j] = PrefabUtility.InstantiatePrefab(TilePrefab) as Tile;
                    _Tiles[i, j].transform.parent = _TileFolder;
                    _Tiles[i, j].transform.position = GridUtils.indexToWorldPos(i, j, TilePrefab.transform);
                    _Tiles[i, j].Coordinates = new Vector2Int(i, j);
                    _Tiles[i, j].gameObject.name = i + "  " + j;
                    _Tiles[i, j].Walkable = tileD.Walkable;
                    _Tiles[i, j].TileType = tileD.TileType;
                    TileEditorFunctions tef = _Tiles[i, j].GetComponent<TileEditorFunctions>();
                    foreach (var interactor in tileD.Interactors) tef.LoadSpawnInteractor(interactor.Split(':')[0], int.Parse(interactor.Split(':')[1]));
                    _Tiles[i, j].Height = tileD.Height;
                    _Tiles[i, j].transform.position = new Vector3(_Tiles[i, j].transform.position.x, tileD.Height, _Tiles[i, j].transform.position.z);
                    _Tiles[i, j].UpdateModel();
                    if (Application.isPlaying)
                        yield return null;
                }
            }
            _IsGenerationOver = true;

            yield return null;
        }
    }
#endif


    public void SpawnTile(int row, int column)
    {
        _Tiles[row, column] = Instantiate(TilePrefab, GridUtils.indexToWorldPos(row, column, TilePrefab.transform), Quaternion.identity, _TileFolder) as Tile;
        _Tiles[row, column].GetComponent<NetworkObject>().Spawn();
    }

    [ShowIf("_AllowGridEdition"), Button, Group("Tile Generation"), Tab("Tile Generation")]
    public void DestroyGrid()
    {
        Tile[] previousTiles = FindObjectsOfType<Tile>();
        if (previousTiles != null && previousTiles.Length > 0)
        {
#if UNITY_EDITOR
            foreach (Tile tile in previousTiles) DestroyImmediate(tile.gameObject);
#else
            foreach (Tile tile in previousTiles) Destroy(tile.gameObject);
#endif
        }
    }

#if UNITY_EDITOR
    [ShowIf("_AllowGridEdition"), Button, Group("Tile Generation"), Tab("Tile Generation")]
    public void InstantiateGrid()
    {
        DestroyGrid();
        _Tiles = new Tile[TileRows, TileColumns];
        for (int i = 0; i < _Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < _Tiles.GetLength(1); j++)
            {
                _Tiles[i, j] = PrefabUtility.InstantiatePrefab(TilePrefab) as Tile;
                _Tiles[i, j].transform.parent = _TileFolder;
                _Tiles[i, j].transform.position = GridUtils.indexToWorldPos(i, j, TilePrefab.transform);
                _Tiles[i, j].Coordinates = new Vector2Int(i, j);
                _Tiles[i, j].gameObject.name = i + "  " + j;
                _Tiles[i, j].UpdateModel(); 
            }
        }
    }



    [ShowIf("_AllowGridEdition"), Button, Group("Tile Generation"), Tab("Tile Generation")]
    public void UpdateGrid()
    {
        Tile[] previousTiles = _Tiles.Cast<Tile>().ToArray();
        _Tiles = new Tile[TileRows, TileColumns];
        for (int i = 0; i < _Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < _Tiles.GetLength(1); j++)
            {
                var coordinates = new Vector2Int(i, j);
                Tile tile = previousTiles.Where(m => m.Coordinates == coordinates).FirstOrDefault();
                if (tile != null)
                {
                    _Tiles[i, j] = tile;
                }
                else
                {
                    _Tiles[i, j] = PrefabUtility.InstantiatePrefab(TilePrefab) as Tile;
                    _Tiles[i, j].transform.parent = _TileFolder;
                    _Tiles[i, j].transform.position = GridUtils.indexToWorldPos(i, j, TilePrefab.transform);
                    _Tiles[i, j].Coordinates = new Vector2Int(i, j);
                    _Tiles[i, j].gameObject.name = i + "  " + j;
                    _Tiles[i, j].UpdateModel();
                }

            }
        }

        Tile[] tiles = previousTiles.Where(m=> m != null && !GridUtils.TContain(Tiles, m)).ToArray();
        foreach(Tile tile in tiles) DestroyImmediate(tile.gameObject);
    }

    [Button, Group("Tile Generation"), Tab("Tile Generation")]
    public void DebugTiles()
    {
        foreach(Tile tile in _Tiles)
        {
            Debug.Log(tile.name);
        }
    }
#endif

    [Button, Group("Tile Generation"), Tab("Tile Generation")]
    private void GetTilesReferences()
    {
        Tile[] currentTiles = FindObjectsOfType<Tile>();
        int maxRow = currentTiles.Max(m => m.Coordinates.x) + 1;
        int maxColumn = currentTiles.Max(m => m.Coordinates.y) + 1;
        _Tiles = new Tile[maxRow, maxColumn];
        for (int i = 0; i < maxRow; i++)
        {
            for (int j = 0; j < maxColumn; j++)
            {
                _Tiles[i, j] = currentTiles.Where(m => m.Coordinates.x == i && m.Coordinates.y == j).FirstOrDefault();
            }
        }

    }
}

