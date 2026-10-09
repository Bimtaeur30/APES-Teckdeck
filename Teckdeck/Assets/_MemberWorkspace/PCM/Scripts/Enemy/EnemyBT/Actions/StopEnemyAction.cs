using Enemy;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop Enemy", story: "Stop [Enemy]", category: "Action/Combat", id: "1be2cf95ab8861c149cb61d3cdf83a13")]
    public partial class StopEnemyAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;

        protected override Status OnStart()
        {
            if (Enemy.Value.NavMovement != null && Enemy.Value == null)
                return Status.Failure;
            Enemy.Value.NavMovement.StopImmediately();
            return Status.Success;
        }
    }
}