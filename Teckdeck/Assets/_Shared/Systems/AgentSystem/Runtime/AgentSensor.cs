using Enemy.Interface;
using ModuleSystem;
using System.Collections;
using UnityEngine;

namespace Assets._Shared.Systems.AgentSystem
{
    public class AgentSensor : MonoBehaviour, IModule, ISensor
    {
        [SerializeField] private LayerMask whatIsTarget;
        [SerializeField] private LayerMask whatIsObstacle;
        [SerializeField] private int maxColliderCount = 5;

        private ModuleOwner _owner;
        private Collider[] _colliderResults;

        public Collider[] ColliderResults => _colliderResults;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            Debug.Assert(maxColliderCount > 0, $"최대 컬라이더 수는 0보다 커야 합니다.: {gameObject}");
            _colliderResults = new Collider[maxColliderCount];
        }

        public bool IsTargetInSight(Transform targetTrm)
        {
            Vector3 targetPosition = targetTrm.position;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            float distance = direction.magnitude;
            if (Physics.Raycast(transform.position, direction.normalized,
                    out RaycastHit hit, distance, whatIsObstacle))
            {
                Debug.Log(hit.collider.gameObject.name);
                return false; //시야를 장애물이 막고 있음.
            }

            return true;
        }

        public bool IsTargetInViewAngle(Transform targetTrm, float viewAngle)
        {
            Vector3 direction = targetTrm.position - transform.position;
            direction.y = 0;
            float angle = Vector3.Angle(transform.forward, direction);
            return angle <= viewAngle * 0.5f;
        }
        public bool IsTargetInViewRadius(Transform targetTrm, float viewRadius)
            => (targetTrm.position - transform.position).sqrMagnitude <= viewRadius * viewRadius;
        public int FindTargetsInRadius(float viewRadius)
            => Physics.OverlapSphereNonAlloc(transform.position, viewRadius, _colliderResults, whatIsTarget);
    }
}