using ModuleSystem;
using UnityEngine;
using UnityEngine.AI;

public class NavMovement : MonoBehaviour , IModule , INavMovement
{
    public NavMeshAgent NavAgent { get; private set; }

    public Vector3 Velocity
    {
        get => NavAgent != null ? NavAgent.velocity : Vector3.zero;
        set
        {
            if (NavAgent != null)
                NavAgent.velocity = value;
        }
    }

    public float Speed 
    {
        get => NavAgent != null ? NavAgent.speed : 0f;
        set
        {
            if (NavAgent != null)
                NavAgent.speed = value;
        }
    }
    public bool IsStopped
    { 
        get => NavAgent != null && NavAgent.isStopped;
        set
        {
            if (NavAgent != null)
                NavAgent.isStopped = value;
        }
    }
    public bool IsArrived => NavAgent != null
        && (!NavAgent.pathPending && NavAgent.remainingDistance < NavAgent.stoppingDistance);

    public void Initialize(ModuleOwner owner)
    {
        NavAgent = owner.GetComponent<NavMeshAgent>();
        Debug.Assert(NavAgent != null, "네비게이션 무브 컴포넌트는 반드시 NavAgent를 필요로 합니다.");
    }

    public void SetDestination(Vector3 destination)
    {
        NavAgent.SetDestination(destination);
    }

    public void StopImmediately()
    {
        NavAgent.ResetPath();
        NavAgent.velocity = Vector3.zero;
    }
}
