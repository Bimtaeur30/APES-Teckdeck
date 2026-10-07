using JTH.RegistryTest;
using UnityEngine;

namespace _MemeberWorkspace.JTH.Scripts.RegistryTest
{
    public class RegistryTestFireball : RegistryTestSkill
    {
        [SerializeReference] private float explosionRadius = 3f;
        [SerializeReference] private GameObject effectPrefab;

        public float ExplosionRadius => explosionRadius;
        public GameObject EffectPrefab => effectPrefab;
    }
}
