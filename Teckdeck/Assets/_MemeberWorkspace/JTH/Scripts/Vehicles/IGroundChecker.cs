using System;

namespace JTH.Vehicles
{
    public interface IGroundChecker
    {
        bool IsGrounded { get; }
        event Action OnGrounded;
    }
}
