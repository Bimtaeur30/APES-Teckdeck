using System;
using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class JumpState : AbstractState, IStateEnter<MovementEnterContainer>
{
    private Player player;

    public JumpState(ModuleOwner owner, int stateClipHash = 0) : base(owner, stateClipHash)
    {
        player = owner as Player;
    }

    public void EnterWith(MovementEnterContainer data, float duration)
    {
        if (!player.MovementModule.JumpStart(data, HandleOnJumpLand))
            Transition.ChangeState((int)StateEnum.Idle);
    }

    public override void Enter(float transitionDuration, int layerIndex = 0)
    {
        base.Enter(transitionDuration, layerIndex);
        player.MovementModule.ResumeJump();
    }

    private void HandleOnJumpLand()
    {
        Transition.ChangeState((int)StateEnum.Idle);
    }
}
