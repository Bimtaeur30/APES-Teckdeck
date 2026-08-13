namespace AnimatorSystem
{
    public interface IAnimatorRenderer
    {
        void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
        void RenderClipIfNotPlaying(int clipHash);
    }
}