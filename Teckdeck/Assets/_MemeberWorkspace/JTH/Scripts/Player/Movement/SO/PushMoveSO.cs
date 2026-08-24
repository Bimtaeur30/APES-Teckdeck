using UnityEngine;

namespace JTH.Player.Movement.SO
{
    [CreateAssetMenu(fileName = "PushMoveSO", menuName = "Player/Movement/PushMoveSO", order = 1)]
    public class PushMoveSO : ScriptableObject
    {
        [field: SerializeField] public float PushPower { get; private set; } = 15f;
    }
}
