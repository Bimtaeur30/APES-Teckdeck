using UnityEngine;

namespace JTH.Vehicles.Movement.SO
{
    [CreateAssetMenu(fileName = "BrakeMoveSO", menuName = "Player/Movement/BrakeMoveSO", order = 2)]
    public class BrakeMoveSO : ScriptableObject
    {
        [field: SerializeField] public float BrakeDecel { get; private set; } = 15f;
        [field: SerializeField] public float StoppedSpeed { get; private set; } = 1f;
    }
}
