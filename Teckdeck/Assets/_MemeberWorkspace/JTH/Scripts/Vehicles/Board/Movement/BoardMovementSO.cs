using UnityEngine;

namespace JTH.Vehicles.Board.Movement
{
    [CreateAssetMenu(fileName = "MovementSO", menuName = "Player/Movement/MovementSO", order = 0)]
    public class BoardMovementSO : ScriptableObject
    {
        [field: Header("Turn Settings")]
        [field: SerializeField] public float DriftTurnSpeed { get; private set; } = 2.5f;
        [field: SerializeField] public float DoDriftTurnSpeed { get; private set; } = 2f;
        [field: SerializeField] public float MaxTurnSpeed { get; private set; } = 100f;
        [field: SerializeField] public float Decay { get; private set; } = 6.3f;
        [field: SerializeField] public float RotationThreshold { get; private set; } = 1f;
        [field: Header("Resistance")]
        [field: SerializeField] public float BaseDecel { get; private set; } = 3f;
        [field: SerializeField] public float BaseDecelThreshold { get; private set; } = 5f;
        [field: Header("Band Settings")]
        [field: SerializeField] public float StoppedSpeed { get; private set; } = 1f;
        [field: SerializeField] public float TuckSpeed { get; private set; } = 20f;
    }
}
