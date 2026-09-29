using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class UIPointerEventRelay : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    private readonly List<UIToggleComponent> _listeners = new();

    public void Register(UIToggleComponent listener)
    {
        if (listener != null && !_listeners.Contains(listener))
            _listeners.Add(listener);
    }

    public void Unregister(UIToggleComponent listener)
    {
        _listeners.Remove(listener);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIToggleComponent[] listeners = _listeners.ToArray();
        foreach (UIToggleComponent listener in listeners)
        {
            if (listener != null && listener.isActiveAndEnabled)
                listener.HandlePointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIToggleComponent[] listeners = _listeners.ToArray();
        foreach (UIToggleComponent listener in listeners)
        {
            if (listener != null && listener.isActiveAndEnabled)
                listener.HandlePointerExit(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIToggleComponent[] listeners = _listeners.ToArray();
        foreach (UIToggleComponent listener in listeners)
        {
            if (listener != null && listener.isActiveAndEnabled)
                listener.HandlePointerClick(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UIToggleComponent[] listeners = _listeners.ToArray();
        foreach (UIToggleComponent listener in listeners)
        {
            if (listener != null && listener.isActiveAndEnabled)
                listener.HandlePointerDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIToggleComponent[] listeners = _listeners.ToArray();
        foreach (UIToggleComponent listener in listeners)
        {
            if (listener != null && listener.isActiveAndEnabled)
                listener.HandlePointerUp(eventData);
        }
    }
}
