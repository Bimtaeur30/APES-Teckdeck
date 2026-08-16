using System;

namespace JTH.Player.Movement
{
    public interface IJumpable
    {
        public bool CanJump { get; }

        public Action OnJumpEnded { get; set; }
        bool IsJumping { get; }
        public void Jump();
    }
}
