using _Shared.Systems.FsmSystem.Runtime;
using ModuleSystem;
using UnityEngine;

public class WalkState : AbstractState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public WalkState(ModuleOwner owner, int stateClipHash = 0) : base(owner, stateClipHash)
    {
    }
    
    
}
