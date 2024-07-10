using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEditor;
using UnityEngine;
using static Tile;

public class TileEditorFunctions : MonoBehaviour
{
    [SerializeField] private MeshRenderer _MainRenderer;
    [SerializeField] private MeshFilter _MainFilter;
    [SerializeField] private Transform _SpawnPositions;
    [SerializeField] private SO_TileData _TileGeneralData;

    private Tile _Tile;

    public SO_TileData TileGeneralData { get => _TileGeneralData; }

    private void OnEnable()
    {

        GetComponent<TileNetworkOperations>()._onNetworkConnect += OnNetworkConnection;
    }

    private void OnDisable()
    {
        GetComponent<TileNetworkOperations>()._onNetworkConnect -= OnNetworkConnection;
    }

    private void OnNetworkConnection()
    {
        _MainRenderer.transform.Rotate(0, UnityEngine.Random.Range(0, 6) * 60, 0);
    }

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
            case ETileTypes.Neutral: mat[1] = TileGeneralData.plaineMatTop; mat[0] = TileGeneralData.plaineMatBottom; break;
            case ETileTypes.Wood: mat = new Material[1]; mat[0] = TileGeneralData.woodMat; break;
            case ETileTypes.Rock: mat = new Material[1]; mat[0] = TileGeneralData.rockMat; break;
            case ETileTypes.Sand: mat = new Material[1]; mat[0] = TileGeneralData.desertMatBottom; break;
            case ETileTypes.Undegradable:
                mat = new Material[1]; mat[0] = TileGeneralData.undegradableMatBottom;
                mat[1] = TileGeneralData.undegradableMat; break;
            default: mat[1] = TileGeneralData.plaineMatTop; mat[0] = TileGeneralData.plaineMatBottom; break;
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
            case ETileTypes.Neutral: mat[1] = TileGeneralData.fplaineMatTop; mat[0] = TileGeneralData.fplaineMatBottom; break;
            case ETileTypes.Wood: mat = new Material[1]; mat[0] = TileGeneralData.fwoodMat; break;
            case ETileTypes.Rock: mat = new Material[1]; mat[0] = TileGeneralData.frockMat; break;
            case ETileTypes.Sand: mat = new Material[1]; mat[0] = TileGeneralData.fdesertMatBottom; break;
            case ETileTypes.Undegradable:
                mat = new Material[1]; mat[0] = TileGeneralData.fundegradableMatBottom;
                mat[1] = TileGeneralData.fundegradableMat; break;
            default: mat[1] = TileGeneralData.fplaineMatTop; mat[0] = TileGeneralData.fplaineMatBottom; break;
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
            case ETileTypes.Neutral: mesh = TileGeneralData.defaultMesh; break;
            case ETileTypes.Wood: mesh = TileGeneralData.woodMesh; break;
            case ETileTypes.Rock: mesh = TileGeneralData.rockMesh; break;
            case ETileTypes.Sand: mesh = TileGeneralData.sandMesh; break;
            case ETileTypes.Undegradable: mesh = TileGeneralData.undegradableMesh; break;
            default: mesh = TileGeneralData.defaultMesh; break;
        }

        return mesh;
    }

#if UNITY_EDITOR
    public void SpawnInteractors(EMaterialType materialType, ESpawnPositions spawnPositions)
    {
        int myInt = Convert.ToInt32(spawnPositions);
        bool[] bools = GetSpawnPositions(myInt);
        for (int i = 0; i < bools.Length; i++)
        {
            if (bools[i])
            {
                Transform tr = _SpawnPositions.GetChild(i);
                if(tr.transform.childCount > 0)
                {
                    foreach (Transform t in tr) DestroyImmediate(t.gameObject);
                }
                Interactor itemToSpawn = GetInteractorByMaterialType(materialType);
                if (itemToSpawn == null) return;

                Interactor interactor = PrefabUtility.InstantiatePrefab(itemToSpawn) as Interactor;
                interactor.transform.parent = tr;
                interactor.transform.localPosition = Vector3.zero;
                interactor.MaterialType = materialType;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i <= 8 ; i++)
        {
            Transform tr = _SpawnPositions.GetChild(i);
            Gizmos.DrawIcon(tr.position , $"Icon_{i}.png", false);
        }
    }
#endif
    private Interactor GetInteractorByMaterialType(EMaterialType materialType)
    {
        switch (materialType)
        {
            case EMaterialType.Neutral: return null;
            case EMaterialType.Wood:
                 return _TileGeneralData.treePrefab;
            case EMaterialType.Rock:
                 return _TileGeneralData.rockPrefab;
            default: return _TileGeneralData.treePrefab;
        }
    }

    private bool[] GetSpawnPositions(int flagNum)
    {
        bool[] bools = new bool[7];
        if (flagNum == -1)
        {
            for (int i = 0; i < bools.Length; i++) bools[i] = true;
            return bools;
        }
        int moduloNum = 64;
        for (int i = bools.Length - 1; i >= 0; i--)
        {
            bools[i] = IterateThroughFlags(ref flagNum, moduloNum);
            moduloNum /= 2;
        }

        return bools;
    }

    private bool IterateThroughFlags(ref int iteratedNum, int moduloNum)
    {
        if (iteratedNum >= moduloNum)
        {
            iteratedNum -= moduloNum;
            return true;
        }
        else return false;
    }

#if UNITY_EDITOR
    public void LoadSpawnInteractor(string materialName, int childIndex)
    {
        Interactor prefab = GetInteractorByMaterialType(Enum.Parse<EMaterialType>(materialName));
        Transform tr = _SpawnPositions.GetChild(childIndex);
        Interactor interactor = PrefabUtility.InstantiatePrefab(prefab) as Interactor;
        interactor.transform.parent = tr;
        interactor.transform.localPosition = Vector3.zero;
    }
#endif
}
