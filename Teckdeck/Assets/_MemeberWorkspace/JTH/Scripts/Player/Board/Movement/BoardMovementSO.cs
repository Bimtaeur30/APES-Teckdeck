using UnityEngine;

namespace JTH.Player.Board.Movement
{
    [CreateAssetMenu(fileName = "BoardMovementSO", menuName = "Board/MovementSO", order = 0)]
    public class BoardMovementSO : ScriptableObject
    {
        [field: Header("Push Settings")]
        [field: SerializeField] public float PushPower { get; private set; } =  12f;
        [field: Header("Turn Settings")]
        [field: SerializeField] public float MaxTurnSpeed { get; private set; } =  10f;
        [field: SerializeField] public float Decay { get; private set; } =  15f;
        [field: Header("Side Grip")]
        [field: SerializeField] public float SnapAngle { get; private set; } = 20f;
        [field: SerializeField] public float BreakAngle { get; private set; } = 45f;
        [field: SerializeField] public float BreakSpeed { get; private set; } = 8f;
        [field: SerializeField] public float GripDecay { get; private set; } = 15f;
        [field: SerializeField] public float KineticDecel { get; private set; } = 8f;
        [field: Header("Resistance")]
        [field: SerializeField] public float BaseDecel { get; private set; } = 2f;
        [field: SerializeField] public float BaseDecelThreshold { get; private set; } = 5f;
        [field: SerializeField] public float BrakeDecel { get; private set; } = 10f; 
        [field: Header("Band Settings")]
        [field: SerializeField] public float StoppedSpeed { get; private set; } =  1f;
        [field: SerializeField] public float TuckSpeed { get; private set; } =  8f;
        [field: Header("Jump Settings")]
        [field: SerializeField] public AnimationCurve JumpYVelocity { get; private set; }
        [field: SerializeField] public float JumpCooldown { get; private set; } = 0.75f;
        [field: SerializeField] public float JumpDuration { get; private set; } = 1f;
        [field: SerializeField] public float JumpPower { get; private set; }
    }
}
