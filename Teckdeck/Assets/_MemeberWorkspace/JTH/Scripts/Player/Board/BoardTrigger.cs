using System;
using ModuleSystem;
using UnityEngine;

namespace JTH.Player.Board
{
    public class BoardTrigger : MonoBehaviour, IModule
    {
        public event Action OnPushStarted;
        public event Action OnPushEnded;
        public event Action OnAnimationEnd;
        public event Action OnBrakeStarted;

        public void Initialize(ModuleOwner owner)
        {
        }

        private void AnimationEndTrigger() => OnAnimationEnd?.Invoke();
        private void PushStartTrigger() => OnPushStarted?.Invoke();
        private void PushEndTrigger() => OnPushEnded?.Invoke();
        private void BrakeStartTrigger() => OnBrakeStarted?.Invoke();
    }
}
