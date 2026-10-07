using Enemy;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Rotate To Target", story: "[Enemy] rotate to [Target]", category: "Action", id: "1d8a991b7cd0f691b19d82b9d43f447e")]
public partial class RotateToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<float> RotateSpeed = new(10f);
    [SerializeReference] public BlackboardVariable<float> RotateDuration = new(0.4f);

    private float _startTime;
    protected override Status OnStart()
    {
        if (Enemy.Value == null || Target.Value == null)
            return Status.Failure;

        _startTime = Time.time;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_startTime + RotateDuration.Value < Time.time)
            return Status.Success;

        Vector3 direction = (Target.Value.transform.position - Enemy.Value.transform.position);
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        Enemy.Value.transform.rotation
            = Quaternion.Slerp(Enemy.Value.transform.rotation, targetRotation, RotateSpeed.Value * Time.deltaTime);
        return Status.Running;
    }
}

