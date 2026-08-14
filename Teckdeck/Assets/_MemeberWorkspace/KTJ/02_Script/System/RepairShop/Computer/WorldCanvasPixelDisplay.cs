using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class WorldCanvasPixelDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas sourceCanvas;
    [SerializeField] private Camera captureCamera;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private RawImage outputImage;

    [Header("Capture Alignment")]
    [SerializeField, Min(0.01f)] private float cameraDistance = 1f;
    [SerializeField] private Color backgroundColor = Color.black;

    private void OnEnable()
    {
        ApplyReferences();
        AlignCaptureCamera();
    }

    private void LateUpdate()
    {
        AlignCaptureCamera();
    }

    private void OnValidate()
    {
        cameraDistance = Mathf.Max(0.01f, cameraDistance);
        ApplyReferences();
        AlignCaptureCamera();
    }

    private void ApplyReferences()
    {
        if (captureCamera != null)
        {
            captureCamera.targetTexture = renderTexture;
            captureCamera.orthographic = true;
            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = backgroundColor;
            captureCamera.allowHDR = false;
            captureCamera.allowMSAA = false;
        }

        if (renderTexture != null)
        {
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.wrapMode = TextureWrapMode.Clamp;
        }

        if (outputImage != null)
        {
            outputImage.texture = renderTexture;
            outputImage.raycastTarget = false;
        }
    }

    private void AlignCaptureCamera()
    {
        if (sourceCanvas == null || captureCamera == null || renderTexture == null)
            return;

        if (sourceCanvas.renderMode != RenderMode.WorldSpace)
            return;

        if (outputImage != null && outputImage.transform.IsChildOf(sourceCanvas.transform))
            return;

        RectTransform sourceRect = sourceCanvas.transform as RectTransform;
        if (sourceRect == null)
            return;

        Vector3[] corners = new Vector3[4];
        sourceRect.GetWorldCorners(corners);

        Vector3 center = (corners[0] + corners[2]) * 0.5f;
        float worldWidth = Vector3.Distance(corners[0], corners[3]);
        float worldHeight = Vector3.Distance(corners[0], corners[1]);
        float textureAspect = renderTexture.width / (float)renderTexture.height;

        Transform canvasTransform = sourceCanvas.transform;
        captureCamera.transform.SetPositionAndRotation(
            center - canvasTransform.forward * cameraDistance,
            Quaternion.LookRotation(canvasTransform.forward, canvasTransform.up));

        captureCamera.aspect = textureAspect;
        captureCamera.orthographicSize = Mathf.Max(
            worldHeight * 0.5f,
            worldWidth / (2f * textureAspect));
        captureCamera.nearClipPlane = 0.01f;
        captureCamera.farClipPlane = cameraDistance + 1f;
    }
}
