using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tile;

public class TileEditorFunctions : MonoBehaviour
{
    [SerializeField] private SO_TileData _TileGeneralData;
    [SerializeField] private MeshRenderer _MainRenderer;
    [SerializeField] private MeshFilter _MainFilter;
    private Tile _Tile;

    public void UpdateModel()
    {

            _Tile = GetComponent<Tile>();
            if (getCorrespondingMat(_Tile.TileType) != null)
            {
                Material[] mats = getCorrespondingMat(_Tile.TileType);
                if (!Application.isPlaying) _MainRenderer.sharedMaterials = mats;
                else _MainRenderer.materials = mats;
            }

            if (getCorrespondingMesh(_Tile.TileType) != null)
            {
                _MainFilter.sharedMesh = getCorrespondingMesh(_Tile.TileType);
            }
    }

    public Material[] getCorrespondingMat(ETileTypes tType)
    {
        Material[] mat = new Material[2];

        if (!_Tile.Walkable)
        {
            _MainRenderer.enabled = false;
            return null;
        }

        switch (tType)
        {
            case ETileTypes.Neutral: mat[1] = _TileGeneralData.plaineMatTop; mat[0] = _TileGeneralData.plaineMatBottom; break;
            case ETileTypes.Wood: mat = new Material[1]; mat[0] = _TileGeneralData.woodMat; break;
            case ETileTypes.Rock: mat = new Material[1]; mat[0] = _TileGeneralData.rockMat; break;
            case ETileTypes.Sand: mat = new Material[1]; mat[0] = _TileGeneralData.desertMatBottom; break;
            case ETileTypes.Undegradable:
                mat = new Material[1]; mat[0] = _TileGeneralData.undegradableMatBottom;
                mat[1] = _TileGeneralData.undegradableMat; break;
            default: mat[1] = _TileGeneralData.plaineMatTop; mat[0] = _TileGeneralData.plaineMatBottom; break;
        }

        if (_Tile.Walkable && !_MainRenderer.enabled)
        {
            _MainRenderer.enabled = true;
        }

        return mat;
    }

    public Material[] getFadeCorrespondingMat(ETileTypes tType)
    {
        Material[] mat = new Material[2];
        if (!_Tile.Walkable)
        {
            _MainRenderer.enabled = false;
            return null;
        }

        switch (tType)
        {
            case ETileTypes.Neutral: mat[1] = _TileGeneralData.fplaineMatTop; mat[0] = _TileGeneralData.fplaineMatBottom; break;
            case ETileTypes.Wood: mat = new Material[1]; mat[0] = _TileGeneralData.fwoodMat; break;
            case ETileTypes.Rock: mat = new Material[1]; mat[0] = _TileGeneralData.frockMat; break;
            case ETileTypes.Sand: mat = new Material[1]; mat[0] = _TileGeneralData.fdesertMatBottom; break;
            case ETileTypes.Undegradable:
                mat = new Material[1]; mat[0] = _TileGeneralData.fundegradableMatBottom;
                mat[1] = _TileGeneralData.fundegradableMat; break;
            default: mat[1] = _TileGeneralData.fplaineMatTop; mat[0] = _TileGeneralData.fplaineMatBottom; break;
        }

        if (_Tile.Walkable && !_MainRenderer.enabled)
        {
            _MainRenderer.enabled = true;
        }

        return mat;
    }

    public Mesh getCorrespondingMesh(ETileTypes tType)
    {
        Mesh mesh;
        if (!_Tile.Walkable) return null;


        switch (tType)
        {
            case ETileTypes.Neutral: mesh = _TileGeneralData.defaultMesh; break;
            case ETileTypes.Wood: mesh = _TileGeneralData.woodMesh; break;
            case ETileTypes.Rock: mesh = _TileGeneralData.rockMesh; break;
            case ETileTypes.Sand: mesh = _TileGeneralData.sandMesh; break;
            case ETileTypes.Undegradable: mesh = _TileGeneralData.undegradableMesh; break;
            default: mesh = _TileGeneralData.defaultMesh; break;
        }

        return mesh;
    }
}
