using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable 
{
    Transform Transform { get;}

    public void OnGrab(Transform newParent);

    public void OnRelease();

    public void TriggerHightlight(bool Activate);
}
