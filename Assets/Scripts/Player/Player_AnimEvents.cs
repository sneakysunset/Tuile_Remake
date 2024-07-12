using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimEvents : MonoBehaviour
{
    FMOD.Studio.EventInstance _WalkEvent;
    [SerializeField] ParticleSystem _PSysWalkingR, P_SysWalkingL;
    Player _Player;

    private void Start()
    {
        _Player = GetComponentInParent<Player>();
    }

    public void OnWalkEvent()
    {
        return;
        _WalkEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Tuile/Character/Actions/Move");
        _WalkEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(/*P_SysWalkingL.transform*/transform));
        _WalkEvent.setParameterByNameWithLabel("GroundType", _Player.GroundType);
        _WalkEvent.start();
       // pSysWalkingR.Play();
    }

    public void OnHitEvent()
    {
        return;
    }
}
