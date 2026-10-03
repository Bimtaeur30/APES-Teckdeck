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
    
    private bool isWall = false;
    private bool isJumping;
    private Action OnJumpLand;

    public void Initialize(ModuleOwner owner)
    {
        // ModuleOwner의 공통 초기화 후 Player가 Configure에서 필요한 참조를 전달한다.
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
        JumpUpdate();
    }

    private void JumpUpdate()
    {
        if (!isJumping) return;

        Vector3 movement = _currentMovementVector.Direction * (jumpForce * Time.deltaTime);
        float distance = movement.magnitude;
        if (distance <= 0f) return;

        Vector3 direction = movement / distance;
        Vector3 center = _bodyCollider.transform.TransformPoint(_bodyCollider.center);
        Vector3 scale = _bodyCollider.transform.lossyScale;
        float radius = _bodyCollider.radius * Mathf.Max(
            Mathf.Abs(scale.x), Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

        float allowedDistance = distance;
        bool wallAhead = false;

        // SphereCast는 시작 위치에서 이미 겹친 가까운 벽을 놓칠 수 있다.
        if (Physics.Raycast(center, direction, out RaycastHit rayHit,
                distance + radius, wallLayer, QueryTriggerInteraction.Ignore))
        {
            allowedDistance = Mathf.Min(allowedDistance,
                Mathf.Max(0f, rayHit.distance - radius));
            wallAhead = true;
        }

        if (Physics.SphereCast(center, radius, direction, out RaycastHit sphereHit,
                distance, wallLayer, QueryTriggerInteraction.Ignore))
        {
            allowedDistance = Mathf.Min(allowedDistance,
                Mathf.Max(0f, sphereHit.distance));
            wallAhead = true;
        }

        _body.position += direction * allowedDistance;

        if (!wallAhead) return;

        isJumping = false;
        OnWallEnter();
    }

    public void JumpStart(MovementVector vector, Action onJumpLand)
    {
        if (!isWall || isJumping) return;

        _currentMovementVector = vector;
        isJumping = true;
        OnJumpLand = onJumpLand;
    }

    private void OnWallEnter()
    {
        Debug.Log("OnWallEnter");
        OnJumpLand?.Invoke();
    }

    private void CheckIsWall()
    {
        Vector3 center = _bodyCollider.transform.TransformPoint(_bodyCollider.center);
        if (Physics.OverlapSphereNonAlloc(center, overlapSphereRadius, colliders , wallLayer) > 0)
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
}
