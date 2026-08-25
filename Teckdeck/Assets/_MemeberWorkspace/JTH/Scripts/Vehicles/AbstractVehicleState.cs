using _Shared.Systems.FsmSystem.Runtime;
using JTH.Vehicles.InputSystem;
using JTH.Vehicles.Movement;
using ModuleSystem;

namespace JTH.Vehicles
{
    public abstract class AbstractVehicleState : AbstractState
    {
        protected PlayerInputSO PlayerInput;
        protected VehicleController Vehicle;
        protected readonly IControlMovement ControlMovement;
        protected const float InputDeadZone = 0.1f; //입력을 안받는 임계값
        
        protected AbstractVehicleState(ModuleOwner agent, int stateClipHash) : base(agent, stateClipHash)
        {
            Vehicle = (VehicleController)agent;
            PlayerInput = Vehicle.PlayerInput;
            ControlMovement = agent.GetModule<IControlMovement>();
        }
    }
}