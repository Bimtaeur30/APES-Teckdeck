using System;
using UnityEngine;

public interface IPlayerMovementModule
{
    void Configure(Transform body, SphereCollider bodyCollider);
    bool JumpStart(MovementEnterContainer movementEnter, Action onJumpLand);
    void PauseJump();
    void ResumeJump();
    bool MoveFlightStep(Vector3 direction, float distance);
    void SetJumpDirection(Vector3 direction);
}
