using AnimatorSystem;
using CombatSystem;
using Systems.AgentSystem;
using UnityEngine;

public class TestEnemySkill : AbstractEnemySkill
{
    [SerializeField] private HashDataSO skillAnimParam;
    [SerializeField] private float crossFadeDuration = 0.15f;

    private AgentTrigger _trigger;

    public override void InitializeSkill(ISkillModule skillModule)
    {
        base.InitializeSkill(skillModule);
        _trigger = _ownerEnemy.GetModule<AgentTrigger>();
        Debug.Assert(_trigger != null, $"애니메이션 트리거가 있어야 근접 공격스킬을 사용가능 : {gameObject}");
    }

    public override bool CanUseSkill(GameObject target = null)
    {
        if (target == null)
            return false;

        Vector3 distanceToTarget = target.transform.position - _ownerEnemy.transform.position;
        distanceToTarget.y = 0;

        return NormalizedCooldown >= 1f && distanceToTarget.sqrMagnitude <= SkillData.skillRange * SkillData.skillRange;
    }

    public override void UseSkill(GameObject target = null)
    {
        base.UseSkill(target);
        _renderer.PlayClip(skillAnimParam.HashValue, 0, crossFadeDuration);
        _trigger.OnAnimationEnd += StopSkill;
    }

    public override void StopSkill()
    {
        base.StopSkill();
        _trigger.OnAnimationEnd -= StopSkill;
    }
}
