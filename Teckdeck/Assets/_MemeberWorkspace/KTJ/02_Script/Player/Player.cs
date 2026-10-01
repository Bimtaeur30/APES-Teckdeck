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
    [field:SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
    [field:SerializeField] public Rigidbody Rigidbody { get; private set; }
    [SerializeField] private LineRenderer lineRenderer;
    private IPlayerMovementModule _playerMovementModule;
    
    protected override void InitializeModules()
    {
        base.InitializeModules();
        _playerMovementModule = GetModule<IPlayerMovementModule>();
    }

    protected override void Awake()
    {
        base.Awake();
        PlayerInputSO.OnJumpKeyPressed += HandleOnJumpKeyPressed;
        PlayerInputSO.OnMovementChange += HandleOnMovementChanged;
    }

    private void Update()
    {
        Vector3 dir = GetMouseDirection();
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 100f))
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

    private void HandleOnJumpKeyPressed()
    {
        Vector3 dir = GetMouseDirection();
        _playerMovementModule.JumpTo(dir);
    }

    private Vector3 GetMouseDirection()
    {
        Vector2 screenMousePos = PlayerInputSO.GetMouseScreenPosition();
        Ray ray = Camera.main.ScreenPointToRay(screenMousePos);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance); // GetPoint는 앞에서 구한 거리만큼 광선을 따라 이동해 실제 교차점의 월드 좌표를 구한다.
            mouseWorldPos.y = transform.position.y;
            return (mouseWorldPos - transform.position).normalized;
        }

        return Vector3.zero;
    }

    private void HandleOnMovementChanged(Vector2 obj)
    {
        // 일단 비워둔다.
    }
}
