using System;
using ModuleSystem;
using UnityEngine;

public class PlayerMovementModule : MonoBehaviour, IModule, IPlayerMovementModule
{   
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float overlapSphereRadius = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    private readonly Collider[] colliders = new Collider[8];
    private Transform _body;
    private SphereCollider _bodyCollider;
    private MovementVector _currentMovementVector;
    private float _currentJumpCharge = 1f;
    
    private bool isWall = false;
    private bool isJumping;
    private bool isJumpPaused;
    private bool landedDuringFlight;
    private Action onJumpLand;
    private Player player;

    public void Initialize(ModuleOwner owner)
    {
        // ModuleOwner의 공통 초기화 후 Player가 Configure에서 필요한 참조를 전달한다
        player = owner as Player;
    }

    public void Configure(Transform body, SphereCollider bodyCollider)
    {
        if (body == null) throw new ArgumentNullException(nameof(body));
        if (bodyCollider == null) throw new ArgumentNullException(nameof(bodyCollider));

        _body = body;
        _bodyCollider = bodyCollider;
    }

    private void Update()
    {
        CheckIsWall();
        if (!isJumpPaused)
            JumpUpdate();
    }

    private void JumpUpdate()
    {
        if (!isJumping) return;

        if (landedDuringFlight)
        {
            landedDuringFlight = false;
            CompleteJump();
            return;
        }

        Vector3 movement = _currentMovementVector.Direction * (jumpForce * _currentJumpCharge * Time.deltaTime);
        if (MoveWithWallCollision(movement, out Vector3 wallNormal))
        {
            RotateTo(wallNormal);
            CompleteJump();
        }
    }

    public void PauseJump()
    {
        if (isJumping)
            isJumpPaused = true;
    }

    public void ResumeJump()
    {
        isJumpPaused = false;
    }

    public bool MoveFlightStep(Vector3 direction, float distance)
    {
        if (!isJumping || !isJumpPaused || direction.sqrMagnitude <= 0f || distance <= 0f)
            return false;

        Vector3 flightDirection = direction.normalized;
        RotateTo(flightDirection);
        bool hitWall = MoveWithWallCollision(flightDirection * distance, out Vector3 wallNormal);
        if (hitWall)
        {
            RotateTo(wallNormal);
            landedDuringFlight = true;
        }

        return hitWall;
    }

    public void SetJumpDirection(Vector3 direction)
    {
        direction.y = 0f;
        if (!isJumping || !isJumpPaused || landedDuringFlight || direction.sqrMagnitude <= 0.0001f)
            return;

        Vector3 normalizedDirection = direction.normalized;
        _currentMovementVector = new MovementVector(normalizedDirection, _currentMovementVector.Distance);
        RotateTo(normalizedDirection);
    }

    private bool MoveWithWallCollision(Vector3 movement, out Vector3 wallNormal)
    {
        wallNormal = default;
        float distance = movement.magnitude;
        if (distance <= 0f) return false;

        Vector3 direction = movement / distance;
        Vector3 center = _bodyCollider.transform.TransformPoint(_bodyCollider.center);
        Vector3 scale = _bodyCollider.transform.lossyScale;
        float radius = _bodyCollider.radius * Mathf.Max(
            Mathf.Abs(scale.x), Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

        float allowedDistance = distance;
        bool wallAhead = false;
        float nearestAllowedDistance = float.MaxValue;

        // SphereCast는 시작 위치에서 이미 겹친 가까운 벽을 놓칠 수 있다.
        if (Physics.Raycast(center, direction, out RaycastHit rayHit,
                distance + radius, wallLayer, QueryTriggerInteraction.Ignore))
        {
            allowedDistance = Mathf.Min(allowedDistance,
                Mathf.Max(0f, rayHit.distance - radius));
            wallAhead = true;
            nearestAllowedDistance = Mathf.Max(0f, rayHit.distance - radius);
            wallNormal = rayHit.normal;
        }

        if (Physics.SphereCast(center, radius, direction, out RaycastHit sphereHit,
                distance, wallLayer, QueryTriggerInteraction.Ignore))
        {
            allowedDistance = Mathf.Min(allowedDistance,
                Mathf.Max(0f, sphereHit.distance));
            wallAhead = true;
            if (sphereHit.distance < nearestAllowedDistance)
                wallNormal = sphereHit.normal;
        }

        _body.position += direction * allowedDistance;

        return wallAhead || TryGetArrivalWallNormal(
            center + direction * allowedDistance, radius, direction, out wallNormal);
    }

    private void CompleteJump()
    {
        isJumping = false;
        isJumpPaused = false;
        onJumpLand?.Invoke();
    }

    public bool JumpStart(MovementEnterContainer movementEnter, Action onJumpLand)
    {
        CheckIsWall();
        if (!isWall || isJumping || movementEnter.Vector.Direction.sqrMagnitude <= 0f) return false;

        _currentMovementVector = movementEnter.Vector;
        _currentJumpCharge = movementEnter.JumpCharge;
        isJumping = true;
        isJumpPaused = false;
        landedDuringFlight = false;
        this.onJumpLand = onJumpLand;
        RotateTo(movementEnter.Vector.Direction); // 진행방향 바라보기
        return true;
    }

    private bool TryGetArrivalWallNormal(Vector3 center, float radius, Vector3 direction, out Vector3 wallNormal)
    {
        wallNormal = default;
        int count = Physics.OverlapSphereNonAlloc(center, radius + 0.01f, colliders,
            wallLayer, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            Vector3 closest = colliders[i].ClosestPoint(center);
            Vector3 awayFromWall = center - closest;
            if (awayFromWall.sqrMagnitude <= 0.000001f ||
                Vector3.Dot(awayFromWall.normalized, direction) >= -0.1f)
                continue;

            wallNormal = awayFromWall.normalized;
            return true;
        }

        return false;
    }

    private void CheckIsWall()
    {
        Vector3 center = _bodyCollider.transform.TransformPoint(_bodyCollider.center);
        if (Physics.OverlapSphereNonAlloc(center, overlapSphereRadius, colliders,
                wallLayer, QueryTriggerInteraction.Ignore) > 0)
        {
            isWall = true;
        }
        else
        {
            isWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isWall ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapSphereRadius);
    }

    private void RotateTo(Vector3 target)
    {
        target.y = 0;
        _body.rotation = Quaternion.LookRotation(target);
    }
}
