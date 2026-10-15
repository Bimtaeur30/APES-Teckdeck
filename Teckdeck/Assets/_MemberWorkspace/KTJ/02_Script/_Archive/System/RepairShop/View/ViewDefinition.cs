using System;
using Unity.Cinemachine;
using UnityEngine;

public enum ViewId
{
    Overview, Monitor, Palm
}

[Serializable]
public class ViewDefinition
{
    public ViewId Id;
    public CinemachineCamera Camera;
    public MonoBehaviour InteractionBehavior;

    public IViewInteraction Interaction => InteractionBehavior as IViewInteraction;
}
