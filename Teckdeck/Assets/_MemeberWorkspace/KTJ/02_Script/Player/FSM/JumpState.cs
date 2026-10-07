using System;
using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class JumpState : AbstractState, IStateEnter<MovementContainer>
{
    private Player player;

    public JumpState(ModuleOwner owner, int stateClipHash = 0) : base(owner, stateClipHash)
    {
        player = owner as Player;
    }

    public void EnterWith(MovementContainer data, float duration)
    {
        if (!player.MovementModule.JumpStart(data, HandleOnJumpLand))
            Transition.ChangeState((int)StateEnum.Idle);
    }

    private void HandleOnJumpLand()
    {
        Transition.ChangeState((int)StateEnum.Idle);
    }
}
