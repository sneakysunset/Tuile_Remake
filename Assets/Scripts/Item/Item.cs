using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour, IGrabbable
{
    private GameObject _Highlight;

    public Transform Transform { get => transform; }

    private Transform _ParentTransform;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => IsSpawned);
        if (IsServer)
        {
            _Highlight = transform.Find("Highlight").gameObject;
        }
    }

    public void OnGrab(Transform newParent)
    {
        _ParentTransform = newParent;
        TriggerHightlight(false);
    }

    public void OnRelease()
    {
        _ParentTransform = null;
    }

    private void Update()
    {
        UpdatePosition();
    }

    [ClientRpc]
    void UpdatePosition()
    {
        transform.position = _ParentTransform.position; 
        transform.rotation = _ParentTransform.rotation;
    }

    
    public void TriggerHightlight(bool Activate)
    {
        if (!IsOwner) return;
        if (Activate)
        {
            _Highlight.SetActive(true);
        }
        else
        {
            _Highlight.SetActive(false);
        }
    }
}
