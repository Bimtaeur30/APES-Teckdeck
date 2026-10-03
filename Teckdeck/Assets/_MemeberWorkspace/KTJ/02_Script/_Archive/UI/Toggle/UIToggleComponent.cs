using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIToggleComponent : MonoBehaviour
{
    [Header("Pointer Input")]
    [SerializeField] private Graphic pointerTarget = null;

    [Header("Playback")]
    [SerializeField] private UITogglePlaybackCondition playbackCondition =
        UITogglePlaybackCondition.Hover;
    [SerializeField, Min(0f)] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutQuad;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool initialState = false;

    private Tween _tween;
    private UIPointerEventRelay _pointerRelay;
    private bool _isOn;

    public bool IsOn => _isOn;

    protected virtual void OnEnable()
    {
        BindPointerTarget();
        _isOn = initialState;
        ApplyImmediate(_isOn);

        if (playbackCondition == UITogglePlaybackCondition.OnEnable)
            SetState(!initialState);
    }

    protected virtual void OnDisable()
    {
        UnbindPointerTarget();
        KillTween();
    }

    protected virtual void OnDestroy()
    {
        KillTween();
    }

    internal void HandlePointerEnter(PointerEventData eventData)
    {
        if (playbackCondition == UITogglePlaybackCondition.Hover)
            SetState(true);
    }

    internal void HandlePointerExit(PointerEventData eventData)
    {
        if (playbackCondition == UITogglePlaybackCondition.Hover)
            SetState(false);
    }

    internal void HandlePointerClick(PointerEventData eventData)
    {
        if (playbackCondition == UITogglePlaybackCondition.Click)
            Toggle();
    }

    internal void HandlePointerDown(PointerEventData eventData)
    {
        if (playbackCondition == UITogglePlaybackCondition.PointerDown)
            SetState(true);
    }

    internal void HandlePointerUp(PointerEventData eventData)
    {
        if (playbackCondition == UITogglePlaybackCondition.PointerDown)
            SetState(false);
    }

    public void Toggle()
    {
        SetState(!_isOn);
    }

    public void SetState(bool isOn)
    {
        _isOn = isOn;
        KillTween();

        if (!isActiveAndEnabled || duration <= 0f)
        {
            ApplyImmediate(isOn);
            return;
        }

        _tween = CreateTween(isOn, duration)
            .SetEase(ease)
            .SetUpdate(useUnscaledTime)
            .OnKill(() => _tween = null);
    }

    private void BindPointerTarget()
    {
        if (!Application.isPlaying ||
            playbackCondition == UITogglePlaybackCondition.OnEnable)
            return;

        Graphic source = pointerTarget != null
            ? pointerTarget
            : GetComponent<Graphic>();

        if (source == null)
        {
            Debug.LogWarning(
                $"{GetType().Name} requires a Pointer Target Graphic for " +
                $"the {playbackCondition} playback condition.",
                this);
            return;
        }

        _pointerRelay = source.GetComponent<UIPointerEventRelay>();
        if (_pointerRelay == null)
            _pointerRelay = source.gameObject.AddComponent<UIPointerEventRelay>();

        _pointerRelay.Register(this);
    }

    private void UnbindPointerTarget()
    {
        if (_pointerRelay == null)
            return;

        _pointerRelay.Unregister(this);
        _pointerRelay = null;
    }

    private void KillTween()
    {
        if (_tween == null)
            return;

        _tween.Kill();
        _tween = null;
    }

    protected abstract void ApplyImmediate(bool isOn);
    protected abstract Tween CreateTween(bool isOn, float tweenDuration);
}
