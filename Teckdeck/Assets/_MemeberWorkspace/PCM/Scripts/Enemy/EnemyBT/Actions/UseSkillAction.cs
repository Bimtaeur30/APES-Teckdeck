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
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private ISkillModule _skillModule;
        private bool _isSkillEnd;

        protected override Status OnStart()
        {
            if (Enemy.Value == null || SkillNumber.Value < 0)
                return Status.Failure;

            ISkillModule skillModule = Enemy.Value.SkillModule;
            if (skillModule == null || !skillModule.CanUseSkill(SkillNumber.Value, Target.Value))
                return Status.Failure;

            _skillModule = skillModule;
            _isSkillEnd = false;
            _skillModule.UseSkill(SkillNumber.Value, Target.Value);
            _skillModule.OnCurrentSkillEnd += HandleSkillEnd;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _isSkillEnd ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (_skillModule == null)
                return;

            _skillModule.OnCurrentSkillEnd -= HandleSkillEnd;
            _skillModule.StopSkillIfNotFinished();
            _skillModule = null;
        }

        private void HandleSkillEnd() => _isSkillEnd = true;
    }
}
