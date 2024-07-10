using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[DeclareTabGroup("Data")]
[DeclareHorizontalGroup("Data/Center Tile")]
[DeclareHorizontalGroup("Data/Undegradable Tile")]
[DeclareHorizontalGroup("Data/Plaine Tile")]
[DeclareHorizontalGroup("Data/Wood Tile")]
[DeclareHorizontalGroup("Data/Rock Tile")]
[DeclareHorizontalGroup("Data/Sand Tile")]
[DeclareHorizontalGroup("Data/FCenter Tile")]
[DeclareHorizontalGroup("Data/FUndegradable Tile")]
[DeclareHorizontalGroup("Data/FPlaine Tile")]
[DeclareHorizontalGroup("Data/FWood Tile")]
[DeclareHorizontalGroup("Data/FRock Tile")]
[DeclareHorizontalGroup("Data/FSand Tile")]
[DeclareHorizontalGroup("Data/Disabled Tile")]
[DeclareHorizontalGroup("Data/DegradationTimer")]

[CreateAssetMenu(fileName = "Tile Data", menuName = "TileData")]
public class SO_TileData : ScriptableObject
{
    #region Parameters
    [Group("Data"), Tab("Parameters")] public float tourbillonSpeed;
    [Group("Data/DegradationTimer"), Tab("Parameters")] public float baseMinTimerDegradation;
    [Group("Data/DegradationTimer"), Tab("Parameters")] public float baseMaxTimerDegradation;
    #endregion
    #region Mesh & Mat
    [Tab("Mesh Mat"), Group("Data/Center Tile")] public Mesh centerMesh;
    [Tab("Mesh Mat"), Group("Data/Center Tile")] public Material centerMat, centerMatBottom;
    [Tab("Fade Mesh Mat"), Group("Data/FCenter Tile")] public Material fcenterMat, fcenterMatBottom;
    [Tab("Mesh Mat"), Group("Data/Undegradable Tile")] public Mesh undegradableMesh;
    [Tab("Mesh Mat"), Group("Data/Undegradable Tile")] public Material undegradableMat, undegradableMatBottom;
    [Tab("Fade Mesh Mat"), Group("Data/FUndegradable Tile")] public Material fundegradableMat, fundegradableMatBottom;
    [Tab("Mesh Mat"), Group("Data/Plaine Tile")] public Mesh defaultMesh;
    [Tab("Mesh Mat"), Group("Data/Plaine Tile")] public Material plaineMatTop, plaineMatBottom;
    [Tab("Fade Mesh Mat"), Group("Data/FPlaine Tile")] public Material fplaineMatTop, fplaineMatBottom;
    [Tab("Mesh Mat"), Group("Data/Wood Tile")] public Mesh woodMesh;
    [Tab("Mesh Mat"), Group("Data/Wood Tile")] public Material woodMat;
    [Tab("Fade Mesh Mat"), Group("Data/FWood Tile")] public Material fwoodMat;
    [Tab("Mesh Mat"), Group("Data/Rock Tile")] public Mesh rockMesh;
    [Tab("Mesh Mat"), Group("Data/Rock Tile")] public Material rockMat;
    [Tab("Fade Mesh Mat"), Group("Data/FRock Tile")] public Material frockMat;
    [Tab("Mesh Mat"), Group("Data/Sand Tile")] public Mesh sandMesh;
    [Tab("Mesh Mat"), Group("Data/Sand Tile")] public Material desertMatTop, desertMatBottom;
    [Tab("Fade Mesh Mat"), Group("Data/FSand Tile")] public Material fdesertMatTop, fdesertMatBottom;
    [Tab("Mesh Mat"), Group("Data/Disabled Tile")] public Material disabledMaterial;
    #endregion
    #region Interactor
    [Group("Data"), Tab("Interactors")] public Interactor treePrefab;
    [Group("Data"), Tab("Interactors")] public Interactor rockPrefab;
    #endregion
}
