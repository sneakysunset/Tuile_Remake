using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using System.Linq;

[DeclareBoxGroup("Item"),DeclareHorizontalGroup("Item/Grp")]
public class Player_ItemSystem : MonoBehaviour
{
    [ShowInInspector, ReadOnly, Group("Item/Grp")] private IGrabbable _HeldItem;
    [ShowInInspector, ReadOnly, Group("Item/Grp")] private List<IGrabbable> _CloseItems;
    [ShowInInspector, ReadOnly, Group("Item/Grp")] private IGrabbable _ClosestItem;
    public IGrabbable ClosestItem { get => _ClosestItem; set => OnClosestItemChange(value); }

    private void Start()
    {
        _CloseItems = new List<IGrabbable>();
    }

    private void OnEnable()
    {
        MonoBehaviourExtension._onSlowUpdate += SlowUpdate;
    }

    private void OnDisable()
    {
        MonoBehaviourExtension._onSlowUpdate -= SlowUpdate;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.parent.TryGetComponent(out IGrabbable item))
        {
            if(!_CloseItems.Contains(item))
            {
                _CloseItems.Add(item);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out IGrabbable item))
        {
            if (_CloseItems.Contains(item))
            {
                _CloseItems.Remove(item);
                if(_ClosestItem == item)
                {
                    ClosestItem = null;
                }
            }
        }
    }

    private void SlowUpdate()
    {
        UpdateClosestItem();
    }

    private void UpdateClosestItem()
    {
        if (_CloseItems != null && _CloseItems.Count > 0)
        {
            ClosestItem = _CloseItems.OrderBy(m => (transform.position - m.Transform.position).magnitude).FirstOrDefault();
        }
    }

    private void OnClosestItemChange(IGrabbable value)
    {
        _ClosestItem?.TriggerHightlight(false);
        _ClosestItem = value;
        _ClosestItem?.TriggerHightlight(true);
    }
}
