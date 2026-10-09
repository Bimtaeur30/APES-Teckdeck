using System;
using FsmSystem;
using ModuleSystem;

namespace KTJ._02_Script.Player.FSM
{
    [Serializable]
    public class JumpState : AbstractState, IStateEnter<MovementContainer>
    {
        private global::Player player;

        // public JumpState(ModuleOwner owner, int stateClipHash = 0) : base(owner, stateClipHash)
        // {
        // }

        public override void InitializeState(ModuleOwner owner)
        {
            base.InitializeState(owner);
        
            player = owner as global::Player;
        }

        public void EnterWith(MovementContainer data, float duration)
        {
            if (!player.MovementModule.JumpStart(data, HandleOnJumpLand))
                Transition.ChangeState((int)PlayerState.Idle);
        }

        private void HandleOnJumpLand()
        {
            Transition.ChangeState((int)PlayerState.Idle);
        }
    }
}
