using _MemeberWorkspace.KTJ._02_Script.Player.InputSystem;
using _Shared.Systems.RegistrySystem.Runtime;
using FsmSystem;
using KTJ._02_Script.Player.FSM;
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
    [SerializeField] private RegistryRuntime registryRuntime;
    
    // 점프차징
    [SerializeField] private float maxJumpMultiplyValue = 3f;
    [field:SerializeField] public ChargingUI ChargingUI;
    
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
        PlayerInputSO.OnJumpKeyReleased += HandleOnJumpKeyReleased;
        PlayerInputSO.OnMovementChange += HandleOnMovementChanged;

        Fsm = new StateMachine(this, registryRuntime);
        Fsm.ChangeState((int)PlayerState.Idle);
    }


    private void Update()
    {
        Fsm.UpdateMachine();
        CalculateJumpCharging();
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

    private bool isJumpCharging = false;
    private float currentJumpCharge = 1f;
    private void CalculateJumpCharging()
    {
        if (isJumpCharging)
        {
            currentJumpCharge = Mathf.Min(maxJumpMultiplyValue,  currentJumpCharge + Time.deltaTime);
            ChargingUI.Charge(currentJumpCharge, maxJumpMultiplyValue);
            Debug.Log(currentJumpCharge);
        }
        else
        {
            currentJumpCharge = 1f;
        }
    }

    private void HandleOnJumpKeyPressed()
    {
        if (Fsm.CurrentState is IdleState)
        {
            isJumpCharging = true;
            
            ChargingUI.Set();
        }
    }

    private void HandleOnJumpKeyReleased()
    {
        if (Fsm.CurrentState is IdleState)
        {
            isJumpCharging = false;
            IdleState idleState = (IdleState)Fsm.CurrentState;
            idleState.StartJump(currentJumpCharge);
            
            ChargingUI.UnSet();
        }
    }

    private void HandleOnMovementChanged(Vector2 obj)
    {
        // 일단 비워둔다.
    }
}
