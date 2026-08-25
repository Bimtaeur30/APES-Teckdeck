using JTH.Vehicles.Board.Movement;
using UnityEngine;

namespace JTH.Vehicles.Movement
{
    public interface IControlMovement
    {
        public bool IsManualMoving { get; set; }
        public BoardSpeedBand SpeedBand { get; }
        public void Turn(float direction);
        public void SetTransform(Vector3 position, Quaternion rotation);
    }
}
