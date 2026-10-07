using Enemy;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Chase To TargetAction", story: "[Enemy] Chase To [Target]", category: "Action/Combat", id: "cef1f76d72d50a4c93d4287694b07022")]
    public partial class ChaseToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private Vector3 _destination; //잦은 목적지 변경시 성능저하가 발생함으로 저장해둔다.
        private INavMovement _navMovement;
        protected override Status OnStart()
        {
            if (Enemy.Value == null && Enemy.Value.NavMovement == null && Target.Value == null)
                return Status.Failure;

            _destination = Target.Value.transform.position;
            _navMovement = Enemy.Value.NavMovement;
            _navMovement.SetDestination(_destination);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null)
                return Status.Failure; //플레이어 사망시에는 실패 처리
            Vector3 newDestination = Target.Value.transform.position;
            float delta = Vector3.Distance(_destination, newDestination);
            if (delta > 1f)
            {
                _destination = newDestination;
                _navMovement.SetDestination(_destination);
            }
            float distanceToTarget = Vector3.Distance(
                Enemy.Value.transform.position,
                Target.Value.transform.position);
            return distanceToTarget <= Enemy.Value.EnemyData.StopDistance  ? Status.Success : Status.Running;
        }
    }
}