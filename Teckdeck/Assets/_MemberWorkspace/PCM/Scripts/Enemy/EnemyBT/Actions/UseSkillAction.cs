using CombatSystem;
using Enemy;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Enemy.BT.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Use Skill", story: "[Enemy] use [SkillNumber] on [Target]", category: "Action/Combat", id: "1332496ababb4d2192ffda8533198013")]
    public partial class UseSkillAction : Action
    {
        [SerializeReference] public BlackboardVariable<AbstractEnemy> Enemy;
        [SerializeReference] public BlackboardVariable<int> SkillNumber;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;

        private ISkillModule _skillModule;
        private bool _isSkillEnd;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || SkillNumber.Value < 0)
            {
                Debug.LogError("UseSkill action :  Enemy 또는 Number가 할당되지 않아 행동이 실패");
                return Status.Failure;
            }

            _skillModule = Enemy.Value.SkillModule;
            _isSkillEnd = false;
            _skillModule.UseSkill(SkillNumber.Value, TargetGameObject.Value);
            _skillModule.OnCurrentSkillEnd += HandleSkillEnd;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _isSkillEnd ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (_skillModule != null)
            {
                _skillModule.OnCurrentSkillEnd -= HandleSkillEnd;
                _skillModule.StopSkillIfNotFinished();
            }
        }

        private void HandleSkillEnd() => _isSkillEnd = true;
    }
}
