using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class IdleState : AbstractState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IdleState(ModuleOwner owner, int stateClipHash) : base(owner, stateClipHash)
    {
        
    }
}
