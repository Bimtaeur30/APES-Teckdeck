using ModuleSystem;
using System;
using UnityEngine;

namespace CombatSystem
{
    public interface ISkillModule
    {
        ModuleOwner Owner { get; }

        event Action OnCurrentSkillEnd; //스킬이 종료되었음을 상태에 알려준다.
        
        //GameObject를 매개변수로 넣은 이유는 타겟팅 스킬을 구현할 때 사용하고자 함이다.
        bool CanUseSkill(int skillIndex, GameObject target = null); //인덱스 스킬이 사용가능한지 체크
        void UseSkill(int skillIndex, GameObject target = null); // 인덱스 스킬을 사용해라.
        void InvokeSkillEnd(); //스킬을 종료시킨다.
        void StopSkillIfNotFinished(); //종료되지 않았다면 종료시켜.
    }
}