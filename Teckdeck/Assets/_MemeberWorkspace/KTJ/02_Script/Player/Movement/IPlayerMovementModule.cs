using System;
using UnityEngine;

public interface IPlayerMovementModule
{
    void Configure(Transform body, SphereCollider bodyCollider);
    bool JumpStart(MovementVector vector, Action onJumpLand);
}
