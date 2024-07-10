using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TriInspector;
using UnityEngine;

[DeclareHorizontalGroup("Degradation Timer"), DeclareHorizontalGroup("Degradation Timer2")]
public class DegradationManager : Singleton<DegradationManager>
{
    [SerializeField, Group("Degradation Timer")] private AnimationCurve _DegradationAnimationCurve;
    [SerializeField, Group("Degradation Timer")] private float _MaxEvolutionTime;
    [SerializeField, Group("Degradation Timer2")] private float _MinEvolutionMult;
    [SerializeField, Group("Degradation Timer2")] private float _MaxEvolutionMult;
    private float _DegradationEvolutionTimer;
    [SerializeField] private  List<DegradationByType> _DegradationByTypeDict;

    public float GetDegradationEvolutionValue()
    {
        return Mathf.Lerp(_MinEvolutionMult, _MaxEvolutionMult, _DegradationAnimationCurve.Evaluate(Mathf.Min(_DegradationEvolutionTimer / _MaxEvolutionTime, 1)));
    }

    public float GetDegradationMultByTileType(ETileTypes tileType)
    {
        for (int i = 0; i < _DegradationByTypeDict.Count; i++)
        {
            if (_DegradationByTypeDict[i].TileTypes == tileType) return _DegradationByTypeDict[i].Multiplier;
        }
        return 1;
    }

    private void Update()
    {
        _DegradationEvolutionTimer += Time.deltaTime;
    }

    [DeclareHorizontalGroup("Grp")]
    [Serializable]
    public struct DegradationByType
    {
        [Group("Grp")] public ETileTypes TileTypes;
        [Group("Grp")] public float Multiplier;
    }
}

