using UnityEngine;

namespace JTH.Player.Movement.SO
{
    [CreateAssetMenu(fileName = "MovementSO", menuName = "Player/Movement/MovementSO", order = 0)]
    public class MovementSO : ScriptableObject
    {
        [field: Header("Turn Settings")]
        [field: SerializeField] public float MaxTurnSpeed { get; private set; } = 50f;
        [field: SerializeField] public float Decay { get; private set; } = 6.3f;
        [field: SerializeField] public float RotationThreshold { get; private set; } = 1f;
        [SerializeField] private float driftMaxTurnSpeedMultiplier = 1.5f;
        public float DriftMaxTurnSpeed => MaxTurnSpeed * driftMaxTurnSpeedMultiplier;
        [SerializeField] private float driftDecayMultiplier = 2f;
        public float DriftDecay => Decay * driftDecayMultiplier;
        [field: Header("Resistance")]
        [field: SerializeField] public float BaseDecel { get; private set; } = 3f;
        [field: SerializeField] public float BaseDecelThreshold { get; private set; } = 5f;
        [field: Header("Band Settings")]
        [field: SerializeField] public float StoppedSpeed { get; private set; } = 1f;
        [field: SerializeField] public float TuckSpeed { get; private set; } = 20f;
    }
}
