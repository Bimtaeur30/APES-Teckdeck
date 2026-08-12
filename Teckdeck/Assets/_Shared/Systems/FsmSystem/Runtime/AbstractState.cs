using UnityEngine;

namespace DevLib.FsmSystem.Runtime
{
    public abstract class AbstractState
    {
        protected GameObject _owner;
        protected StateSO _stateSO;
        public AbstractState(GameObject owner, StateSO stateSO)
        {
            _owner = owner;    
            _stateSO = stateSO;
        }
        
        public virtual void Enter() {}
        public virtual void Update() {}
        public virtual void Exit() {}
    }
}