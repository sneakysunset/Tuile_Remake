using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourExtension : Singleton<MonoBehaviourExtension>
{
    public delegate void SlowUpdateEvent();
    public static SlowUpdateEvent slowUpdate;

    private void Start()
    {
        InvokeRepeating("SlowUpdate", 0f, .5f);
    }

    void SlowUpdate()
    {
        slowUpdate?.Invoke();
    }
}
