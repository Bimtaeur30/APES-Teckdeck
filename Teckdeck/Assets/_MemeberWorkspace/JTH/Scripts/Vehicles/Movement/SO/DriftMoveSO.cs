using UnityEngine;

namespace JTH.Vehicles.Movement.SO
{
    [CreateAssetMenu(fileName = "DriftMoveSO", menuName = "Player/Movement/DriftMoveSO", order = 4)]
    public class DriftMoveSO : ScriptableObject
    {
        [field: SerializeField] public float RecoverySideVelRate { get; private set; } = 0.4f;
        [field: Header("Drift Settings")]
        [field: SerializeField] public float DriftCutAngle { get; private set; } = 5f;
        [field: SerializeField] public float DriftDecel { get; private set; } = 5f;
        [field: SerializeField] public float MinDriftTime { get; private set; } = 0.35f;
    }
}
