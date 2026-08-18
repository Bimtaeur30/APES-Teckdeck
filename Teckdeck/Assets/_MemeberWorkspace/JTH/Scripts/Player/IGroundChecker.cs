using System;

namespace JTH.Player
{
    public interface IGroundChecker
    {
        bool IsGrounded { get; }
        event Action OnGrounded;
    }
}
