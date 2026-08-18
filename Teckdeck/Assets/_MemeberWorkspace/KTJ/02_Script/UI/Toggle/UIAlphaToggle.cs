using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class UIAlphaToggle : UIToggleComponent
{
    [Header("Target")]
    [SerializeField] private CanvasGroup target;

    [Header("Alpha")]
    [SerializeField, Range(0f, 1f)] private float offAlpha;
    [SerializeField, Range(0f, 1f)] private float onAlpha = 1f;

    private void Reset()
    {
        target = GetComponent<CanvasGroup>();
        CaptureCurrentAlpha();
    }

    private void OnValidate()
    {
        if (target == null)
            target = GetComponent<CanvasGroup>();
    }

    protected override void ApplyImmediate(bool isOn)
    {
        if (target != null)
            target.alpha = isOn ? onAlpha : offAlpha;
    }

    protected override Tween CreateTween(bool isOn, float tweenDuration)
    {
        if (target == null)
            return DOVirtual.DelayedCall(0f, () => { });

        return target.DOFade(isOn ? onAlpha : offAlpha, tweenDuration);
    }

    private void CaptureCurrentAlpha()
    {
        if (target == null)
            return;

        offAlpha = target.alpha;
        onAlpha = target.alpha;
    }
}
