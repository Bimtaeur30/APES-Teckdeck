using UnityEngine;

namespace JTH.Player
{
    public interface IControlMovement
    {
        bool CanManualMove { get; set; }
        void SetMovementDirection(Vector2 inputDirection);
        void SetTransform(Vector3 position, Quaternion rotation);
    }
}