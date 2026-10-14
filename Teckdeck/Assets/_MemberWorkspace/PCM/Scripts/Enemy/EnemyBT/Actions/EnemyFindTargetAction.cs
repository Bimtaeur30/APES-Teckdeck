using Enemy;
using Enemy.Interface;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy find Target", story: "[Enemy] find [Target]", category: "Action/Combat", id: "b403cf02fac59634a433d513fc57edc6")]
public partial class EnemyFindTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if (Enemy.Value == null || Enemy.Value.Sensor == null || Target.Value != null)
        {
            return Status.Failure; //이미 타겟이 존재하거나 값이 안들어가 있다면 Fail
        }

        ISensor sensor = Enemy.Value.Sensor;
        int detectCount = sensor.FindTargetsInRadius(Enemy.Value.EnemyData.DetectRadius);

        for (int i = 0; i < detectCount; i++)
        {
            Transform findTarget = sensor.ColliderResults[i].transform;
            if (!sensor.IsTargetInViewAngle(findTarget, Enemy.Value.EnemyData.ViewAngle))
                continue; //시야각에 없거나
            if (!sensor.IsTargetInSight(findTarget))
                continue; //시야에 장애물이 있다면.

            Target.Value = findTarget.gameObject;
            break;
        }
        return Target.Value == null ? Status.Failure : Status.Success;
    }
}

