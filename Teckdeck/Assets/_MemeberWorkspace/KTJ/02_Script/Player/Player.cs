using System;
using _MemeberWorkspace.KTJ._02_Script.Player.InputSystem;
using ModuleSystem;
using UnityEngine;

/*
구조 설계

플레이어(인풋)
- 무브먼트 모듈(점프, 이동)
- 헬스 모듈(체력관리)
*/

public class Player : ModuleOwner
{
    [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
    [SerializeField] private LineRenderer lineRenderer;
    private IPlayerMovementModule _playerMovementModule;
    private Vector3 wallNormal;

    protected override void InitializeModules()
    {
        base.InitializeModules();
        _playerMovementModule = GetModule<IPlayerMovementModule>();
        _playerMovementModule.Configure(transform, GetComponent<SphereCollider>());
    }

    protected override void Awake()
    {
        base.Awake();
        PlayerInputSO.OnJumpKeyPressed += HandleOnJumpKeyPressed;
        PlayerInputSO.OnMovementChange += HandleOnMovementChanged;
    }

    private void Update()
    {
        MovementVector vector;
        bool getMouseMovement = TryGetMouseMovement(out vector);
        if (getMouseMovement && Physics.Raycast(transform.position, vector.Direction, out RaycastHit hit, 100f) && IsEnableJumpAngle(vector.Direction))
        {
            float distance = hit.distance;
            Vector3 hitPoint = hit.point;

            // 여기에 라인랜더러로 거리 미리보기 표시하기.
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, hitPoint);
            lineRenderer.SetPosition(1, transform.position);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private bool IsEnableJumpAngle(Vector3 direction)
    {
        // 벽 노말과 점프하고자 하는 방향을 내적해서 
        bool isWithin90 = Vector3.Dot(wallNormal.normalized, direction.normalized) > 0f;
        return isWithin90;
    }

    private void HandleOnJumpKeyPressed()
    {
        MovementVector vector;
        bool getMouseMovement = TryGetMouseMovement(out vector);
        if (IsEnableJumpAngle(vector.Direction) && getMouseMovement)
        {
            _playerMovementModule.JumpStart(vector);
        }
    }

    private bool TryGetMouseMovement(out MovementVector movement)
    {
        Vector2 screenMousePos = PlayerInputSO.GetMouseScreenPosition();
        Ray ray = Camera.main.ScreenPointToRay(screenMousePos);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (!plane.Raycast(ray, out float rayDistance))
        {
            movement = default; 
            return false;
        }

        Vector3 mouseWorldPos = ray.GetPoint(rayDistance);
        mouseWorldPos.y = transform.position.y;

        Vector3 offset = mouseWorldPos - transform.position;
        movement = new MovementVector(offset.normalized, offset.magnitude);
        return true;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            bool didFindWall = false;
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.GetContact(i).normal;

                if (!didFindWall)
                    if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.3f)
                    {
                        wallNormal = normal;
                        didFindWall = true;
                    }
            }
        }
    }

    private void HandleOnMovementChanged(Vector2 obj)
    {
        // 일단 비워둔다.
    }
}
