using System;
using UnityEngine;

namespace CombatSystem
{
    public class RayDamageCaster : AbstractDamageCaster
    {
        public enum CastType
        {
            Ray, Sphere, Box
        }
        [SerializeField] private CastType castType = CastType.Sphere;
        [SerializeField] private Vector3 boxSize = Vector3.one;
        [SerializeField, Range(0.5f, 4f)] private float casterRadius = 1f;
        [SerializeField, Range(0, 1.5f)] private float casterInterpolation = 0.5f;
        [SerializeField, Range(0, 5f)] private float castingRange = 1f;

        [SerializeField] private bool isDebugMode = false;
        
        public override bool CastDamage(Vector3 position, Vector3 direction, SkillDataSO skillData)
        {
            //뒤로 보간만큼 빼준뒤에 실행
            Vector3 startPosition = position - direction * casterInterpolation * 2f;

            RaycastHit hit = default;
            bool isHit = castType switch
            {
                CastType.Ray => Physics.Raycast(startPosition, direction, out hit, castingRange, whatIsTarget),
                CastType.Sphere => Physics.SphereCast(startPosition, casterRadius, direction,
                    out hit, castingRange, whatIsTarget),
                CastType.Box => Physics.BoxCast(startPosition, boxSize * 0.5f, direction,
                    out hit, Quaternion.identity, castingRange, whatIsTarget),
                _ => false
            };

            if (isHit && hit.collider != null && hit.collider.TryGetComponent(out IDamageable damageable))
            {
                Debug.Log($"<color=red>Hit. </color> {hit.collider.name}");

                DamageData damageData = new DamageData
                {
                    Attacker = CasterOwner, IsCritical = false, DamageAmount = skillData.baseDamage
                };
                
                Debug.Log($"<color=red>Hit </color> {hit.collider.name} to damage : {damageData.DamageAmount}");
                
                LastHitPoint = damageData.HitPoint = hit.point;
                LastHitNormal = damageData.HitNormal = hit.normal;
                LastHitCritical = damageData.IsCritical;
                
                damageable.ApplyDamage(damageData);
            }
            
            return isHit;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(!isDebugMode) return;
            
            Vector3 startPos = transform.position + transform.forward * -casterInterpolation * 2;
            switch (castType)
            {
                case CastType.Ray:
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(startPos, transform.forward * castingRange);
                    break;
                case CastType.Sphere:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(startPos, casterRadius);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(startPos + transform.forward * castingRange, casterRadius);
                    break;
                case CastType.Box:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(startPos, boxSize);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(startPos + transform.forward * castingRange, boxSize);
                    break;
            }
            Gizmos.color = Color.white;
        }
#endif

    }
}