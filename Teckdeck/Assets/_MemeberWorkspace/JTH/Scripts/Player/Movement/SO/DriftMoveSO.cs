using UnityEngine;

namespace JTH.Player.Movement.SO
{
    [CreateAssetMenu(fileName = "DriftMoveSO", menuName = "Player/Movement/DriftMoveSO", order = 4)]
    public class DriftMoveSO : ScriptableObject
    {
        [field: Header("Side Grip")]
        [field: SerializeField] public float SideSpeedRecoveryRate { get; private set; } = 0.5f;
        [field: Header("Drift Settings")]
        [field: SerializeField] public float DriftCutAngle { get; private set; } = 5f;
        [field: SerializeField] public float DriftDecel { get; private set; } = 5f;
        [field: SerializeField] public float MinDriftTime { get; private set; } = 0.35f;
    }
}
