using AnimatorSystem;

namespace JTH.Player.Movement
{
    public interface IPushable
    {
        public HashDataSO FrontPushHash { get; }
        public HashDataSO BackPushHash { get; }
        public bool IsPushing { get; }
        public void Push(float direction);
        public void EndPush();
    }
}
