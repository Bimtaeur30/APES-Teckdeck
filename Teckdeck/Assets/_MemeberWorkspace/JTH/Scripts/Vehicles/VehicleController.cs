using _Shared.Systems.FsmSystem.Runtime;
using JTH.Vehicles.InputSystem;
using JTH.Vehicles.Movement;
using ModuleSystem;
using UnityEngine;

namespace JTH.Vehicles
{
    public class VehicleController : ModuleOwner
    {
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [SerializeField] private StateListSO playerStates;
        
        private IControlMovement _movement;
        protected StateMachine StateMachine;

        protected override void InitializeModules()
        {
            base.InitializeModules();
            
            StateMachine = new StateMachine(this, playerStates.states); //상태 머신을 생성한다.
            
            _movement = GetModule<IControlMovement>();
            Debug.Assert(_movement != null, "플레이어 이동 관련 모듈이 없습니다.");
        }

        protected override void Start()
        {
            StateMachine.ChangeState(0, transitionDuration: 0); //IDLE상태로
        }

        private void Update()
        {
            StateMachine.UpdateMachine();
        }

        public void ChangeState(int newStateIndex, float transitionDuration)
            => StateMachine.ChangeState(newStateIndex, transitionDuration);
    }
}