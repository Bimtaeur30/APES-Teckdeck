using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class VirtualMouse : MonoBehaviour, IViewInteraction
{
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private RectTransform moveArea;
    [SerializeField] private RectTransform cursor;
    [SerializeField, Min(0f)] private float sensitivity = 1f;

    [Header("World Target")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private Vector2 targetXRange = new(-1f, 1f);
    [SerializeField] private Vector2 targetZRange = new(-1f, 1f);
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertZ;

    private Vector2 _previousMousePosition;
    private Vector3 _targetStartPosition;
    private IViewInteraction _viewInteractionImplementation;
    private bool isEnabled = true;

    private void OnEnable()
    {
        if (Mouse.current != null)
            _previousMousePosition = Mouse.current.position.ReadValue();

        if (targetObject != null)
            _targetStartPosition = targetObject.position;
    }

    private bool TryGetLocalMouseDelta(
        Vector2 previousScreenPosition,
        Vector2 currentScreenPosition,
        out Vector2 localDelta)
    {
        Camera eventCamera = worldCanvas.worldCamera != null
            ? worldCanvas.worldCamera
            : Camera.main;

        bool previousSucceeded =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                moveArea,
                previousScreenPosition,
                eventCamera,
                out Vector2 previousLocalPosition);

        bool currentSucceeded =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                moveArea,
                currentScreenPosition,
                eventCamera,
                out Vector2 currentLocalPosition);

        localDelta = currentLocalPosition - previousLocalPosition;
        return previousSucceeded && currentSucceeded;
    }

    private Vector2 ClampToMoveArea(Vector2 nextPosition)
    {
        Rect areaRect = moveArea.rect;
        Bounds cursorBounds =
            RectTransformUtility.CalculateRelativeRectTransformBounds(moveArea, cursor);

        Vector2 moveDelta = nextPosition - cursor.anchoredPosition;
        float projectedMinX = cursorBounds.min.x + moveDelta.x;
        float projectedMaxX = cursorBounds.max.x + moveDelta.x;
        float projectedMinY = cursorBounds.min.y + moveDelta.y;
        float projectedMaxY = cursorBounds.max.y + moveDelta.y;

        if (cursorBounds.size.x <= areaRect.width)
        {
            if (projectedMinX < areaRect.xMin)
                moveDelta.x += areaRect.xMin - projectedMinX;
            else if (projectedMaxX > areaRect.xMax)
                moveDelta.x -= projectedMaxX - areaRect.xMax;
        }

        if (cursorBounds.size.y <= areaRect.height)
        {
            if (projectedMinY < areaRect.yMin)
                moveDelta.y += areaRect.yMin - projectedMinY;
            else if (projectedMaxY > areaRect.yMax)
                moveDelta.y -= projectedMaxY - areaRect.yMax;
        }

        return cursor.anchoredPosition + moveDelta;
    }

    private void UpdateTargetPosition()
    {
        if (targetObject == null)
            return;

        Rect areaRect = moveArea.rect;
        Bounds cursorBounds =
            RectTransformUtility.CalculateRelativeRectTransformBounds(moveArea, cursor);

        float minCenterX = areaRect.xMin + cursorBounds.extents.x;
        float maxCenterX = areaRect.xMax - cursorBounds.extents.x;
        float minCenterY = areaRect.yMin + cursorBounds.extents.y;
        float maxCenterY = areaRect.yMax - cursorBounds.extents.y;

        float normalizedX = NormalizePosition(cursorBounds.center.x, minCenterX, maxCenterX);
        float normalizedZ = NormalizePosition(cursorBounds.center.y, minCenterY, maxCenterY);

        if (invertX)
            normalizedX = 1f - normalizedX;

        if (invertZ)
            normalizedZ = 1f - normalizedZ;

        Vector3 targetPosition = _targetStartPosition;
        targetPosition.x += CalculateOffset(targetXRange, normalizedX);
        targetPosition.z += CalculateOffset(targetZRange, normalizedZ);
        targetObject.position = targetPosition;
    }

    private static float CalculateOffset(Vector2 range, float normalizedPosition)
    {
        if (normalizedPosition < 0.5f)
            return Mathf.Lerp(range.x, 0f, normalizedPosition * 2f);

        return Mathf.Lerp(0f, range.y, (normalizedPosition - 0.5f) * 2f);
    }

    private static float NormalizePosition(float position, float min, float max)
    {
        if (max <= min)
            return 0.5f;

        return Mathf.InverseLerp(min, max, position);
    }

    private void OnValidate()
    {
        if (cursor == null)
            cursor = transform as RectTransform;
    }

    public void Enter()
    {
        isEnabled = true;
    }

    public void Exit()
    {
        isEnabled = false;
    }

    public void HandleInput()
    {
        if (Mouse.current == null)
            return;

        Vector2 currentMousePosition = Mouse.current.position.ReadValue();

        if (TryGetLocalMouseDelta(
                _previousMousePosition,
                currentMousePosition,
                out Vector2 localDelta))
        {
            Vector2 nextPosition = cursor.anchoredPosition + localDelta * sensitivity;
            cursor.anchoredPosition = ClampToMoveArea(nextPosition);
            UpdateTargetPosition();
        }

        _previousMousePosition = currentMousePosition;
    }
}
