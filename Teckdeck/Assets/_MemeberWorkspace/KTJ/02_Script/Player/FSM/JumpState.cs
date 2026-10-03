using System;
using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class JumpState : AbstractState, IStateEnter<MovementVector>
{
    private Player player;

    public JumpState(ModuleOwner owner, int stateClipHash = 0) : base(owner, stateClipHash)
    {
        player = owner as Player;
    }

    public void EnterWith(MovementVector data, float duration)
    {
        player.MovementModule.JumpStart(data, HandleOnJumpLand);
    }

    private void HandleOnJumpLand()
    {
        Transition.ChangeState((int)StateEnum.Idle);
    }
}
