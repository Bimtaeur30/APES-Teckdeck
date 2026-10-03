using System;
using _MemeberWorkspace.KTJ._02_Script.Player.InputSystem;
using _Shared.Systems.FsmSystem.Runtime;
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
    [field:SerializeField] public LineRenderer LineRenderer { get; private set; }
    [field:SerializeField] public StateMachine Fsm { get; private set; }
    [field:SerializeField] public IPlayerMovementModule MovementModule { get; private set; }
    [field:SerializeField] public Vector3 WallNormal { get; private set; }
    [SerializeField] private StateListSO stateListSO;

    protected override void InitializeModules()
    {
        base.InitializeModules();
        MovementModule = GetModule<IPlayerMovementModule>();
        MovementModule.Configure(transform, GetComponent<SphereCollider>());
    }

    protected override void Awake()
    {
        base.Awake();
        PlayerInputSO.OnJumpKeyPressed += HandleOnJumpKeyPressed;
        PlayerInputSO.OnMovementChange += HandleOnMovementChanged;

        Fsm = new StateMachine(this, stateListSO.states);
        Fsm.ChangeState(0);
    }

    private void Update()
    {
        Fsm.UpdateMachine();
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
                        WallNormal = normal;
                        didFindWall = true;
                    }
            }
        }
    }

    private void HandleOnJumpKeyPressed()
    {
        if (Fsm.CurrentState is IdleState)
        {
            IdleState idleState = (IdleState)Fsm.CurrentState;
            idleState.StartJump();
        }
    }
    

    private void HandleOnMovementChanged(Vector2 obj)
    {
        // 일단 비워둔다.
    }
}
