using UnityEngine;

namespace AnimatorSystem
{
    public interface IAnimatorRenderer
    {
        public Animator Animator { get; }
        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
        public void RenderClipIfNotPlaying(int clipHash,
            float normalizedTime,
            float crossFadeDuration,
            int layerIndex = 0);
    }
}