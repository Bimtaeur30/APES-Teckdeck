using UnityEngine;

namespace Enemy.Interface
{
    public interface ISensor
    {
        Collider[] ColliderResults { get; }
        bool IsTargetInViewAngle(Transform targetTrm, float viewAngle);
        bool IsTargetInSight(Transform targetTrm);
        bool IsTargetInViewRadius(Transform targetTrm, float viewRadius);
        int FindTargetsInRadius(float viewRadius);
    }
}