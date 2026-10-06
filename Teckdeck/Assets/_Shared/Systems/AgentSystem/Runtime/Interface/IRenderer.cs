using UnityEngine;

namespace Enemy.Interface
{
    public interface IRenderer
    {
        Animator Animator { get; }
        void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
        void SetFloat(int idHash, float value);
    }
}