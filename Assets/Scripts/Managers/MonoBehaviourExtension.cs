using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourExtension : Singleton<MonoBehaviourExtension>
{
    public delegate void SlowUpdateEvent();
    public static SlowUpdateEvent _onSlowUpdate;

    public delegate void SessionStartedEvent();
    public static SessionStartedEvent _onSessionStarted;

    public bool IsSessionStarted;

    private void Start()
    {
        InvokeRepeating("SlowUpdate", 0f, .5f);
    }

    void SlowUpdate()
    {
        _onSlowUpdate?.Invoke();
    }

    public void OnSessionStarted()
    {
        IsSessionStarted = true;
        _onSessionStarted?.Invoke();
    }
}
