using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class WorldCanvasPixelDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas sourceCanvas;
    [SerializeField] private RawImage outputImage;

    [Header("Pixel Resolution")]
    [SerializeField, Min(16)] private int textureWidth = 267;
    [SerializeField, Min(16)] private int textureHeight = 165;

    [Header("Capture")]
    [SerializeField, Range(0, 31)] private int captureLayer = 30;
    [SerializeField, Min(0.01f)] private float cameraDistance = 1f;
    [SerializeField] private Color backgroundColor = Color.black;

    private readonly Dictionary<Transform, int> _originalLayers = new();
    private Camera _captureCamera;
    private RenderTexture _renderTexture;

    private void OnEnable()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        StoreAndSetLayer(sourceCanvas.transform);
        CreateRenderTexture();
        CreateCaptureCamera();
        UpdateCaptureCamera();

        outputImage.texture = _renderTexture;
        outputImage.raycastTarget = false;
    }

    private void LateUpdate()
    {
        UpdateCaptureCamera();
    }

    private void OnDisable()
    {
        if (outputImage != null && outputImage.texture == _renderTexture)
            outputImage.texture = null;

        RestoreLayers();
        ReleaseRenderTexture();

        if (_captureCamera != null)
            DestroyRuntimeObject(_captureCamera.gameObject);

        _captureCamera = null;
    }

    private bool ValidateReferences()
    {
        if (sourceCanvas == null || outputImage == null)
        {
            Debug.LogError(
                $"{nameof(WorldCanvasPixelDisplay)} requires a source Canvas and an output RawImage.",
                this);
            return false;
        }

        if (sourceCanvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("The source Canvas must use World Space render mode.", this);
            return false;
        }

        if (outputImage.transform.IsChildOf(sourceCanvas.transform))
        {
            Debug.LogError(
                "The output RawImage must be outside the source Canvas to prevent capture feedback.",
                this);
            return false;
        }

        return true;
    }

    private void CreateRenderTexture()
    {
        _renderTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32)
        {
            name = $"{sourceCanvas.name}_PixelDisplay",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            useMipMap = false,
            autoGenerateMips = false
        };
        _renderTexture.Create();
    }

    private void CreateCaptureCamera()
    {
        GameObject cameraObject = new($"{sourceCanvas.name}_PixelCaptureCamera");
        cameraObject.hideFlags = HideFlags.HideAndDontSave;
        cameraObject.transform.SetParent(transform, false);

        _captureCamera = cameraObject.AddComponent<Camera>();
        _captureCamera.orthographic = true;
        _captureCamera.clearFlags = CameraClearFlags.SolidColor;
        _captureCamera.backgroundColor = backgroundColor;
        _captureCamera.cullingMask = 1 << captureLayer;
        _captureCamera.nearClipPlane = 0.01f;
        _captureCamera.farClipPlane = cameraDistance + 1f;
        _captureCamera.allowHDR = false;
        _captureCamera.allowMSAA = false;
        _captureCamera.targetTexture = _renderTexture;
    }

    private void UpdateCaptureCamera()
    {
        if (_captureCamera == null || sourceCanvas == null)
            return;

        RectTransform sourceRect = sourceCanvas.transform as RectTransform;
        if (sourceRect == null)
            return;

        Vector3[] corners = new Vector3[4];
        sourceRect.GetWorldCorners(corners);

        Vector3 center = (corners[0] + corners[2]) * 0.5f;
        float worldWidth = Vector3.Distance(corners[0], corners[3]);
        float worldHeight = Vector3.Distance(corners[0], corners[1]);
        float textureAspect = textureWidth / (float)textureHeight;

        Transform canvasTransform = sourceCanvas.transform;
        _captureCamera.transform.SetPositionAndRotation(
            center - canvasTransform.forward * cameraDistance,
            Quaternion.LookRotation(canvasTransform.forward, canvasTransform.up));

        _captureCamera.aspect = textureAspect;
        _captureCamera.orthographicSize = Mathf.Max(
            worldHeight * 0.5f,
            worldWidth / (2f * textureAspect));
    }

    private void StoreAndSetLayer(Transform current)
    {
        _originalLayers[current] = current.gameObject.layer;
        current.gameObject.layer = captureLayer;

        for (int i = 0; i < current.childCount; i++)
            StoreAndSetLayer(current.GetChild(i));
    }

    private void RestoreLayers()
    {
        foreach (KeyValuePair<Transform, int> entry in _originalLayers)
        {
            if (entry.Key != null)
                entry.Key.gameObject.layer = entry.Value;
        }

        _originalLayers.Clear();
    }

    private void ReleaseRenderTexture()
    {
        if (_renderTexture == null)
            return;

        _renderTexture.Release();
        DestroyRuntimeObject(_renderTexture);
        _renderTexture = null;
    }

    private static void DestroyRuntimeObject(Object target)
    {
        if (target == null)
            return;

        if (Application.isPlaying)
            Destroy(target);
        else
            DestroyImmediate(target);
    }

    private void OnValidate()
    {
        textureWidth = Mathf.Max(16, textureWidth);
        textureHeight = Mathf.Max(16, textureHeight);
        cameraDistance = Mathf.Max(0.01f, cameraDistance);
    }
}
