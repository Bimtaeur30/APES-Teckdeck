using System.Data;
using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class IdleState : AbstractState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Player player;

    public IdleState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
    {
        player = owner as Player;
    }

    public override void Update()
    {
        base.Update();
        UpdateDrawLine();
    }

    public override void Exit()
    {
        base.Exit();
        player.LineRenderer.positionCount = 0;
    }

    private bool IsEnableJumpAngle(Vector3 direction)
    {
        // 벽 노말과 점프하고자 하는 방향을 내적해서
        bool isWithin90 = Vector3.Dot(player.transform.forward, direction.normalized) >= 0f;
        return isWithin90;
    }
    
    private void UpdateDrawLine()
    {
        MovementVector vector;
        bool getMouseMovement = TryGetMouseMovement(out vector);
        if (getMouseMovement && Physics.Raycast(player.gameObject.transform.position, vector.Direction, out RaycastHit hit, 100f) && IsEnableJumpAngle(vector.Direction))
        {
            float distance = hit.distance;
            Vector3 hitPoint = hit.point;

            // 여기에 라인랜더러로 거리 미리보기 표시하기.
            player.LineRenderer.positionCount = 2;
            player.LineRenderer.SetPosition(0, hitPoint);
            player.LineRenderer.SetPosition(1, player.gameObject.transform.position);
        }
        else
        {
            player.LineRenderer.positionCount = 0;
        }
        
        // 차징UI회전시키기
        player.ChargingUI.RotateTo(vector.Direction);
    }
    
    private bool TryGetMouseMovement(out MovementVector movement)
    {
        Vector2 screenMousePos = player.PlayerInputSO.GetMouseScreenPosition();
        Ray ray = Camera.main.ScreenPointToRay(screenMousePos);
        Plane plane = new Plane(Vector3.up, player.gameObject.transform.position);

        if (!plane.Raycast(ray, out float rayDistance))
        {
            movement = default; 
            return false;
        }

        Vector3 mouseWorldPos = ray.GetPoint(rayDistance);
        mouseWorldPos.y = player.gameObject.transform.position.y;

        Vector3 offset = mouseWorldPos - player.gameObject.transform.position;
        movement = new MovementVector(offset.normalized, offset.magnitude);
        return true;
    }
    

    public void StartJump(float forceMultiplier)
    {
        MovementVector vector;
        bool getMouseMovement = TryGetMouseMovement(out vector);
        if (getMouseMovement && vector.Direction.sqrMagnitude > 0f && IsEnableJumpAngle(vector.Direction))
        {
            Transition.ChangeState((int)StateEnum.Jump, new MovementContainer(vector, forceMultiplier));
        }
    }
}
