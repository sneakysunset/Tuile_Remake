using UnityEditor;
using UnityEngine;

[System.Serializable]
public partial class Tile : MonoBehaviour
{

    [System.Flags]
    public enum ESpawnPositions
    {
        Nothing = 0,
        Pos1 = 1,
        Pos2 = 2,
        Pos3 = 4,
        Pos4 = 8,
        Pos5 = 16,
        Pos6 = 32,
        Pos7 = 64,
        Everything = 0b1111
    }

    [SerializeField] private bool _Walkable = false;
    [SerializeField] private ETileTypes _TileType = ETileTypes.Neutral;
    [SerializeField] private ESpawnPositions _SpawnPositions = ESpawnPositions.Nothing;
    [SerializeField] private Vector2Int _Coordinates;
    private float _Height = 0;

    #region public fields
    public Vector2Int Coordinates { get => _Coordinates; set => _Coordinates = value; }

    private Vector2Int[] _AdjacentTilesCoordinates;
    public Vector2Int[] AdjacentTilesCoordinates { get => _AdjacentTilesCoordinates;}
    public bool Walkable { get => _Walkable; set => _Walkable = value; }
    public ETileTypes TileType { get => _TileType; set => _TileType = value; }
    public ESpawnPositions SpawnPositions { get => _SpawnPositions; set => _SpawnPositions = value; }
    public float Height { get => _Height; set => _Height = value; }
    #endregion


    public void UpdateModel() => GetComponent<TileEditorFunctions>().UpdateModel();

#if UNITY_EDITOR
    void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

    private void _OnValidate()
    {
        if (this == null) return;
        GetComponent<TileEditorFunctions>().UpdateModel();
    }
#endif
}

[CustomEditor(typeof(Tile))]
[System.Serializable, CanEditMultipleObjects]
public class TileEditor : Editor
{
    private Tile _Tile;

    private void OnEnable()
    {
        _Tile = (Tile)target;
    }

    private void OnSceneGUI()
    {
        
    }
}