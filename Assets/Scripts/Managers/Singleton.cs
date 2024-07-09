using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T>: MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There are two Singletons of this type in the scene");
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
}