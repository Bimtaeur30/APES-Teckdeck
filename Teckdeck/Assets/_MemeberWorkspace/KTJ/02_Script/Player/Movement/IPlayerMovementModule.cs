using UnityEngine;

public interface IPlayerMovementModule
{
    void Configure(Transform body, SphereCollider bodyCollider);
    void JumpStart(MovementVector vector);
}
