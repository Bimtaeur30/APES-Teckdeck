using CombatSystem;
using ModuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class EnemySkillModule : MonoBehaviour , IModule , ISkillModule
    {
        public ModuleOwner Owner { get; private set; }
        public AbstractEnemy Enemy { get; private set; }
        public event Action OnCurrentSkillEnd;

        private Dictionary<int, ISkill> _skillDict;
        private ISkill _currentSkill;

        public void Initialize(ModuleOwner owner)
        {
            Owner = owner;
            Enemy = Owner as AbstractEnemy;
            Debug.Assert(Enemy != null, $"적의 스킬 모듈은 반드시 AbstractEnemy의 자식이어야 합니다. : {gameObject}");

            _skillDict = GetComponentsInChildren<ISkill>().ToDictionary(skill => skill.SkillData.skillIndex);

            foreach (ISkill skill in _skillDict.Values)
            {
                skill.InitializeSkill(this);
            }
        }

        public bool CanUseSkill(int skillIndex, GameObject target = null)
        {
            if (_currentSkill is { IsUsing: true })
                return false;

            if (_skillDict.TryGetValue(skillIndex, out ISkill skill))
                return skill.CanUseSkill(target);
            return false;
        }

        public void UseSkill(int skillIndex, GameObject target = null)
        {
            if (_skillDict.TryGetValue(skillIndex, out ISkill skill))
            {
                if (_currentSkill != null)
                {
                    _currentSkill.OnSkillEnd -= InvokeSkillEnd;
                }
                _currentSkill = skill;
                _currentSkill.OnSkillEnd += InvokeSkillEnd;
                skill.UseSkill(target);
            }
        }

        public void InvokeSkillEnd()
        {
            _currentSkill.OnSkillEnd -= InvokeSkillEnd;
            OnCurrentSkillEnd?.Invoke();
            _currentSkill = null;
        }

        public void StopSkillIfNotFinished()
        {
            if (_currentSkill != null)
            {
                _currentSkill.StopSkill();
                _currentSkill = null;
            }
        }
    }
}