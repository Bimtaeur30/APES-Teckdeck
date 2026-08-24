using UnityEngine;

namespace JTH.Player.Movement.SO
{
    [CreateAssetMenu(fileName = "JumpMoveSO", menuName = "Player/Movement/JumpMoveSO", order = 3)]
    public class JumpMoveSO : ScriptableObject
    {
        [field: SerializeField] public AnimationCurve JumpYVelocity { get; private set; }
        [field: SerializeField] public float JumpCooldown { get; private set; } = 0.75f;
        [field: SerializeField] public float JumpDuration { get; private set; } = 0.5f;
        [field: SerializeField] public float JumpPower { get; private set; } = 15f;
    }
}
