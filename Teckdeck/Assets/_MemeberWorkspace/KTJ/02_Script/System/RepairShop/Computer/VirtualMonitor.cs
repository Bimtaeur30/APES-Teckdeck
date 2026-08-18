using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

[DisallowMultipleComponent]
public class VirtualMonitor : MonoBehaviour, IViewInteraction
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
    
    [Header("InOutro Screen")]
    [SerializeField] private CanvasGroup InOutroGroup;
    [SerializeField] private CanvasGroup IntroGroup;
    [SerializeField] private CanvasGroup OutroGroup;

    private Vector3 _targetStartPosition;
    private IViewInteraction _viewInteractionImplementation;
    private bool isEnabled = true;
    private Mouse _physicalMouse;
    private GraphicRaycaster _graphicRaycaster;
    private VirtualMousePointerModule _pointerModule;
    private Graphic[] _cursorGraphics;
    private bool[] _cursorRaycastTargets;
    private bool _isVirtualInputActive;
    private bool _wasLeftButtonPressed;
    private readonly Vector3[] _cursorWorldCorners = new Vector3[4];

    private void OnEnable()
    {
        if (targetObject != null)
            _targetStartPosition = targetObject.position;
    }

    private void OnDisable()
    {
        if (_isVirtualInputActive)
            EndVirtualInput();
    }

    private bool TryGetLocalMouseDelta(
        Vector2 screenPosition,
        Vector2 screenDelta,
        out Vector2 localDelta)
    {
        Camera eventCamera = worldCanvas.worldCamera != null
            ? worldCanvas.worldCamera
            : Camera.main;

        bool previousSucceeded =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                moveArea,
                screenPosition,
                eventCamera,
                out Vector2 previousLocalPosition);

        bool currentSucceeded =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                moveArea,
                screenPosition + screenDelta,
                eventCamera,
                out Vector2 currentLocalPosition);

        localDelta = currentLocalPosition - previousLocalPosition;
        return previousSucceeded && currentSucceeded;
    }

    private Vector2 ClampToMoveArea(Vector2 nextPosition)
    {
        Rect areaRect = moveArea.rect;
        Bounds cursorBounds = GetCursorRectBoundsInMoveArea();

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

    private Bounds GetCursorRectBoundsInMoveArea()
    {
        cursor.GetWorldCorners(_cursorWorldCorners);

        Vector3 firstCorner =
            moveArea.InverseTransformPoint(_cursorWorldCorners[0]);
        Vector3 min = firstCorner;
        Vector3 max = firstCorner;

        for (int i = 1; i < _cursorWorldCorners.Length; i++)
        {
            Vector3 localCorner =
                moveArea.InverseTransformPoint(_cursorWorldCorners[i]);
            min = Vector3.Min(min, localCorner);
            max = Vector3.Max(max, localCorner);
        }

        Bounds bounds = new();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    private void UpdateTargetPosition()
    {
        if (targetObject == null)
            return;

        Rect areaRect = moveArea.rect;
        Bounds cursorBounds = GetCursorRectBoundsInMoveArea();

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

    private void BeginVirtualInput()
    {
        _physicalMouse = Mouse.current;
        _graphicRaycaster = worldCanvas.GetComponent<GraphicRaycaster>();

        if (EventSystem.current != null && _graphicRaycaster != null)
        {
            _pointerModule =
                EventSystem.current.gameObject.AddComponent<VirtualMousePointerModule>();
        }
        else
        {
            Debug.LogWarning(
                $"{nameof(VirtualMonitor)} requires an active EventSystem and a " +
                $"{nameof(GraphicRaycaster)} on the World Space Canvas.",
                this);
        }

        DisableCursorRaycasts();
        _isVirtualInputActive = true;
        _wasLeftButtonPressed = false;
        UpdateVirtualPointerState();
    }

    private void EndVirtualInput()
    {
        if (_pointerModule != null)
        {
            _pointerModule.ClearVirtualPointer();
            _pointerModule.enabled = false;
            Destroy(_pointerModule);
        }

        RestoreCursorRaycasts();
        _pointerModule = null;
        _graphicRaycaster = null;
        _physicalMouse = null;
        _isVirtualInputActive = false;
        _wasLeftButtonPressed = false;
    }

    private void DisableCursorRaycasts()
    {
        _cursorGraphics = cursor.GetComponentsInChildren<Graphic>(true);
        _cursorRaycastTargets = new bool[_cursorGraphics.Length];

        for (int i = 0; i < _cursorGraphics.Length; i++)
        {
            _cursorRaycastTargets[i] = _cursorGraphics[i].raycastTarget;
            _cursorGraphics[i].raycastTarget = false;
        }
    }

    private void RestoreCursorRaycasts()
    {
        if (_cursorGraphics == null)
            return;

        for (int i = 0; i < _cursorGraphics.Length; i++)
        {
            if (_cursorGraphics[i] != null)
                _cursorGraphics[i].raycastTarget = _cursorRaycastTargets[i];
        }

        _cursorGraphics = null;
        _cursorRaycastTargets = null;
    }

    private void UpdateVirtualPointerState()
    {
        if (_pointerModule == null || _graphicRaycaster == null ||
            _physicalMouse == null || !_physicalMouse.added)
            return;

        Camera eventCamera = worldCanvas.worldCamera != null
            ? worldCanvas.worldCamera
            : Camera.main;

        if (eventCamera == null)
            return;

        Vector3 cursorWorldCenter = cursor.TransformPoint(cursor.rect.center);
        Vector2 virtualScreenPosition =
            RectTransformUtility.WorldToScreenPoint(eventCamera, cursorWorldCenter);
        bool isLeftButtonPressed = _physicalMouse.leftButton.isPressed;

        _pointerModule.ProcessVirtualPointer(
            virtualScreenPosition,
            _physicalMouse.scroll.ReadValue(),
            isLeftButtonPressed && !_wasLeftButtonPressed,
            !isLeftButtonPressed && _wasLeftButtonPressed,
            isLeftButtonPressed,
            _graphicRaycaster);

        _wasLeftButtonPressed = isLeftButtonPressed;
    }

    private Sequence _inOutroSequence;
    public void Enter()
    {
        isEnabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (!_isVirtualInputActive)
            BeginVirtualInput();

        _inOutroSequence?.Kill();
        _inOutroSequence = DOTween.Sequence();
        
        InOutroGroup.alpha = 1;
        OutroGroup.alpha = 1;
        IntroGroup.alpha = 0;
        
        _inOutroSequence.Append(IntroGroup.DOFade(1f, 1f));
        _inOutroSequence.AppendInterval(1f);
        _inOutroSequence.Append((InOutroGroup.DOFade(0f, 1f)));
    }

    public void Exit()
    {
        isEnabled = false;

        if (_isVirtualInputActive)
            EndVirtualInput();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _inOutroSequence?.Kill();
        _inOutroSequence = DOTween.Sequence();
        
        InOutroGroup.alpha = 1;
        OutroGroup.alpha = 0;
        IntroGroup.alpha = 0;
        
        _inOutroSequence.Append(OutroGroup.DOFade(1f, 0.5f));
    }

    public void HandleInput()
    {
        if (!isEnabled || Mouse.current == null)
            return;

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector2 screenDelta = Mouse.current.delta.ReadValue();

        if (TryGetLocalMouseDelta(
                screenPosition,
                screenDelta,
                out Vector2 localDelta))
        {
            Vector2 nextPosition = cursor.anchoredPosition + localDelta * sensitivity;
            cursor.anchoredPosition = ClampToMoveArea(nextPosition);
            UpdateTargetPosition();
        }

        UpdateVirtualPointerState();
    }
}

internal sealed class VirtualMousePointerModule : BaseInputModule
{
    private readonly List<RaycastResult> _raycastResults = new();
    private PointerEventData _pointerData;
    private bool _isProcessing;
    private bool _clearRequested;

    public override void Process()
    {
    }

    public override bool ShouldActivateModule()
    {
        return false;
    }

    public void ProcessVirtualPointer(
        Vector2 screenPosition,
        Vector2 scrollDelta,
        bool pressedThisFrame,
        bool releasedThisFrame,
        bool isPressed,
        GraphicRaycaster graphicRaycaster)
    {
        _isProcessing = true;

        if (_pointerData == null)
        {
            _pointerData = new PointerEventData(eventSystem)
            {
                pointerId = -100,
                button = PointerEventData.InputButton.Left
            };
        }

        _pointerData.delta = screenPosition - _pointerData.position;
        _pointerData.position = screenPosition;
        _pointerData.scrollDelta = scrollDelta;

        _raycastResults.Clear();
        graphicRaycaster.Raycast(_pointerData, _raycastResults);
        _pointerData.pointerCurrentRaycast = FindFirstRaycast(_raycastResults);

        GameObject currentTarget = _pointerData.pointerCurrentRaycast.gameObject;
        HandlePointerExitAndEnter(_pointerData, currentTarget);

        if (pressedThisFrame)
            ProcessPress(currentTarget);

        if (isPressed)
            ProcessDrag();

        if (releasedThisFrame)
            ProcessRelease(currentTarget);

        if (scrollDelta.sqrMagnitude > 0f)
        {
            GameObject scrollHandler =
                ExecuteEvents.GetEventHandler<IScrollHandler>(currentTarget);
            ExecuteEvents.Execute(
                scrollHandler,
                _pointerData,
                ExecuteEvents.scrollHandler);
        }

        _isProcessing = false;
        if (_clearRequested)
            ClearVirtualPointerNow();
    }

    public void ClearVirtualPointer()
    {
        if (_isProcessing)
        {
            _clearRequested = true;
            return;
        }

        ClearVirtualPointerNow();
    }

    private void ClearVirtualPointerNow()
    {
        _clearRequested = false;

        if (_pointerData == null)
            return;

        ExecuteEvents.Execute(
            _pointerData.pointerPress,
            _pointerData,
            ExecuteEvents.pointerUpHandler);

        if (_pointerData.pointerDrag != null && _pointerData.dragging)
        {
            ExecuteEvents.Execute(
                _pointerData.pointerDrag,
                _pointerData,
                ExecuteEvents.endDragHandler);
        }

        HandlePointerExitAndEnter(_pointerData, null);
        _pointerData = null;
        _raycastResults.Clear();
    }

    private void ProcessPress(GameObject currentTarget)
    {
        _pointerData.eligibleForClick = true;
        _pointerData.delta = Vector2.zero;
        _pointerData.dragging = false;
        _pointerData.useDragThreshold = true;
        _pointerData.pressPosition = _pointerData.position;
        _pointerData.pointerPressRaycast = _pointerData.pointerCurrentRaycast;

        GameObject selectHandler =
            ExecuteEvents.GetEventHandler<ISelectHandler>(currentTarget);
        if (eventSystem.currentSelectedGameObject != selectHandler)
            eventSystem.SetSelectedGameObject(null, _pointerData);

        GameObject pressedTarget = ExecuteEvents.ExecuteHierarchy(
            currentTarget,
            _pointerData,
            ExecuteEvents.pointerDownHandler);
        GameObject clickTarget =
            ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentTarget);

        if (pressedTarget == null)
            pressedTarget = clickTarget;

        float time = Time.unscaledTime;
        _pointerData.clickCount =
            pressedTarget == _pointerData.lastPress &&
            time - _pointerData.clickTime < 0.3f
                ? _pointerData.clickCount + 1
                : 1;
        _pointerData.clickTime = time;
        _pointerData.pointerPress = pressedTarget;
        _pointerData.rawPointerPress = currentTarget;
        _pointerData.pointerClick = clickTarget;
        _pointerData.pointerDrag =
            ExecuteEvents.GetEventHandler<IDragHandler>(currentTarget);

        if (_pointerData.pointerDrag != null)
        {
            ExecuteEvents.Execute(
                _pointerData.pointerDrag,
                _pointerData,
                ExecuteEvents.initializePotentialDrag);
        }
    }

    private void ProcessDrag()
    {
        if (!_pointerData.IsPointerMoving() || _pointerData.pointerDrag == null)
            return;

        if (!_pointerData.dragging && ShouldStartDrag())
        {
            ExecuteEvents.Execute(
                _pointerData.pointerDrag,
                _pointerData,
                ExecuteEvents.beginDragHandler);
            _pointerData.dragging = true;
        }

        if (!_pointerData.dragging)
            return;

        if (_pointerData.pointerPress != _pointerData.pointerDrag)
        {
            ExecuteEvents.Execute(
                _pointerData.pointerPress,
                _pointerData,
                ExecuteEvents.pointerUpHandler);
            _pointerData.eligibleForClick = false;
            _pointerData.pointerPress = null;
            _pointerData.rawPointerPress = null;
        }

        ExecuteEvents.Execute(
            _pointerData.pointerDrag,
            _pointerData,
            ExecuteEvents.dragHandler);
    }

    private bool ShouldStartDrag()
    {
        if (!_pointerData.useDragThreshold)
            return true;

        float threshold = eventSystem.pixelDragThreshold;
        return (_pointerData.pressPosition - _pointerData.position).sqrMagnitude >=
               threshold * threshold;
    }

    private void ProcessRelease(GameObject currentTarget)
    {
        ExecuteEvents.Execute(
            _pointerData.pointerPress,
            _pointerData,
            ExecuteEvents.pointerUpHandler);

        GameObject clickHandler =
            ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentTarget);

        if (_pointerData.pointerClick == clickHandler &&
            _pointerData.eligibleForClick)
        {
            ExecuteEvents.Execute(
                _pointerData.pointerClick,
                _pointerData,
                ExecuteEvents.pointerClickHandler);
        }

        if (_pointerData.pointerDrag != null && _pointerData.dragging)
        {
            ExecuteEvents.ExecuteHierarchy(
                currentTarget,
                _pointerData,
                ExecuteEvents.dropHandler);
            ExecuteEvents.Execute(
                _pointerData.pointerDrag,
                _pointerData,
                ExecuteEvents.endDragHandler);
        }

        _pointerData.eligibleForClick = false;
        _pointerData.pointerPress = null;
        _pointerData.rawPointerPress = null;
        _pointerData.pointerClick = null;
        _pointerData.dragging = false;
        _pointerData.pointerDrag = null;
    }
}
