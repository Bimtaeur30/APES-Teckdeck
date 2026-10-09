using UnityEngine;

namespace JTH.RegistryTest
{
    //레지스트리 에디터 검증용. 확인이 끝나면 지워도 된다.
    public abstract class RegistryTestSkill : MonoBehaviour
    {
        [SerializeField] protected float cooldown = 1f;
        [SerializeField, Range(0f, 10f)] protected float skillRange = 2f;
    }
}
