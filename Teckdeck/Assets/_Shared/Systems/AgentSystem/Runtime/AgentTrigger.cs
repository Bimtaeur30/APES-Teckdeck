using ModuleSystem;
using System;
using UnityEngine;

namespace Systems.AgentSystem
{
    public class AgentTrigger : MonoBehaviour, IModule
    {
        private event Action OnAnimationEnd;
        private event Action OnDamageCast;

        public void Initialize(ModuleOwner owner)
        {
            //모듈 사용의 목적
        }

        private void AnimationEndTrigger() => OnAnimationEnd?.Invoke();
        private void DamageCastTrigger() => OnDamageCast?.Invoke();
    }
}