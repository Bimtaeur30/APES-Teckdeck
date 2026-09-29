using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class UIColorToggle : UIToggleComponent
{
    [Header("Target")]
    [SerializeField] private Graphic target;

    [Header("Color")]
    [SerializeField] private Color offColor = Color.white;
    [SerializeField] private Color onColor = Color.white;

    private void Reset()
    {
        target = GetComponent<Graphic>();
        CaptureCurrentColor();
    }

    private void OnValidate()
    {
        if (target == null)
            target = GetComponent<Graphic>();
    }

    protected override void ApplyImmediate(bool isOn)
    {
        if (target != null)
            target.color = isOn ? onColor : offColor;
    }

    protected override Tween CreateTween(bool isOn, float tweenDuration)
    {
        if (target == null)
            return DOVirtual.DelayedCall(0f, () => { });

        return target.DOColor(isOn ? onColor : offColor, tweenDuration);
    }

    private void CaptureCurrentColor()
    {
        if (target == null)
            return;

        offColor = target.color;
        onColor = target.color;
    }
}
