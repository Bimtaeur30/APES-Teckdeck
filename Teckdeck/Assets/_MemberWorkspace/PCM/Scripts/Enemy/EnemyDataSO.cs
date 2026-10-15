using UnityEngine;

namespace Enemy
{
    public enum EnemyDataField
    {
        DetectRadius,
        ViewAngle,
        StopDistance
    }
    [CreateAssetMenu(fileName = "Enemy data", menuName = "Agent/Enemy data", order = 35)]
    public class EnemyDataSO : ScriptableObject
    {
        [field: SerializeField] public float DetectRadius { get; set; } = 5f;
        [field: SerializeField] public float ViewAngle { get; set; } = 160f;
        [field: SerializeField] public float StopDistance { get; set; } = 1.2f;

        public float GetFieldValue(EnemyDataField fieldEnum) => fieldEnum switch
        {
            EnemyDataField.DetectRadius => DetectRadius,
            EnemyDataField.ViewAngle => ViewAngle,
            EnemyDataField.StopDistance => StopDistance,
            _ => 0
        };
    }
}