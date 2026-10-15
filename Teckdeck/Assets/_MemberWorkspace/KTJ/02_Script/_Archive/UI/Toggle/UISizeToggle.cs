using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class UISizeToggle : UIToggleComponent
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Width / Height")]
    [SerializeField] private Vector2 offSize = new(100f, 100f);
    [SerializeField] private Vector2 onSize = new(120f, 120f);

    private void Reset()
    {
        target = transform as RectTransform;
        CaptureCurrentSize();
    }

    private void OnValidate()
    {
        if (target == null)
            target = transform as RectTransform;
    }

    protected override void ApplyImmediate(bool isOn)
    {
        SetSize(isOn ? onSize : offSize);
    }

    protected override Tween CreateTween(bool isOn, float tweenDuration)
    {
        Vector2 destination = isOn ? onSize : offSize;
        return DOTween.To(GetSize, SetSize, destination, tweenDuration);
    }

    private Vector2 GetSize()
    {
        return target != null ? target.rect.size : Vector2.zero;
    }

    private void SetSize(Vector2 size)
    {
        if (target == null)
            return;

        target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    private void CaptureCurrentSize()
    {
        if (target == null)
            return;

        offSize = target.rect.size;
        onSize = offSize;
    }
}
