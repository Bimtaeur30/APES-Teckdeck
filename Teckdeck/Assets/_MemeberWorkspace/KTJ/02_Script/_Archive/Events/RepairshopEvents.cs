using CoreSystem;
using UnityEngine;

public static class RepairshopEvents
{
    public static readonly ChangeViewEvent ChangeViewEvent = new();
}

public class ChangeViewEvent : GameEvent
{
    public ViewId ViewId;
    public ChangeViewEvent Init(ViewId viewId)
    {
        ViewId = viewId;
        return this;
    }
}
