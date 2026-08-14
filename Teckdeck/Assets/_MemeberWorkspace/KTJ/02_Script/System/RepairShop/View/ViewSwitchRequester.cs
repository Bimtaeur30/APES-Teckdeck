using System;
using CoreSystem;
using UnityEngine;

public class ViewSwitchRequester : MonoBehaviour
{
    [SerializeField] private ViewId targetViewId;
    [SerializeField] private EventChannelSO repairshopChannel;

    public void SwitchViewRequest()
    {
        repairshopChannel.RaiseEvent(RepairshopEvents.ChangeViewEvent.Init(targetViewId));
    }
}
