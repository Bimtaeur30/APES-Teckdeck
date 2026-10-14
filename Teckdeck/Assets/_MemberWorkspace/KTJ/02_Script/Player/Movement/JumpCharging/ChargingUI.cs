using System;
using UnityEngine;
using UnityEngine.UI;

public class ChargingUI : MonoBehaviour
{
    [SerializeField] private RectTransform  chargingImage;
    [SerializeField] private float maxChargeUILength;
    private bool onActivate = false;

    public void Charge(float current, float max)
    {
        float progress = max > 1f ? Mathf.Clamp01((current - 1f) / (max - 1f)) : 0f;
        float chargeValue = progress * maxChargeUILength;
        chargingImage.sizeDelta = new Vector2(chargeValue, chargingImage.sizeDelta.y);
    }

    public void RotateTo(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion roation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = roation;
        }
    }
    
    public void Set()
    {
        onActivate = true;
        chargingImage.sizeDelta = new Vector2(0, chargingImage.sizeDelta.y);
        chargingImage.gameObject.SetActive(true);
    }
    public void UnSet()
    {
        onActivate = false;
        chargingImage.sizeDelta = new Vector2(0, chargingImage.sizeDelta.y);
        chargingImage.gameObject.SetActive(false);
    }
}
