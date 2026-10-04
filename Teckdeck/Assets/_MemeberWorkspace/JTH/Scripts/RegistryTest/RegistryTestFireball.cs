using JTH.RegistryTest;
using UnityEngine;

namespace _MemeberWorkspace.JTH.Scripts.RegistryTest
{
    public class RegistryTestFireball : RegistryTestSkill
    {
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private GameObject effectPrefab;

        public float ExplosionRadius => explosionRadius;
        public GameObject EffectPrefab => effectPrefab;
    }
}
