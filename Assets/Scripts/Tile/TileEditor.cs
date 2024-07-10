/*#if UNITY_EDITOR
using TriInspector;
using UnityEditor;
[DrawWithTriInspector, CustomEditor(typeof(Tile))]
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
#endif*/