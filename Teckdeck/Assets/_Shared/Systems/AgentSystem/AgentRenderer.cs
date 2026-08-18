using AnimatorSystem;
using ModuleSystem;
using UnityEngine;

namespace Systems.AgentSystem
{
    public class AgentRenderer : MonoBehaviour, IModule, IAnimatorRenderer
    {
        public Animator Animator { get; private set; }
        
        public void Initialize(ModuleOwner owner)
        {
            Animator = GetComponent<Animator>();
        }

        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0)
        {
            //Play, CrossFade, CrossFadeFixedTime
            Animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
        }

        public void RenderClipIfNotPlaying(int clipHash,
            float normalizedTime,
            float crossFadeDuration,
            int layerIndex = 0)
        {
            AnimatorStateInfo info = Animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (info.shortNameHash == clipHash)
                return;
            
            PlayClip(clipHash, normalizedTime, crossFadeDuration, layerIndex);
        }
    }
}