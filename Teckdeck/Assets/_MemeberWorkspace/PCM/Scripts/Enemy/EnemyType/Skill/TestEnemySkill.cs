using System.Collections;
using AnimatorSystem;
using CombatSystem;
using Systems.AgentSystem;
using UnityEngine;

public class TestEnemySkill : AbstractEnemySkill
{
    [SerializeField] private HashDataSO skillAnimParam;
    [SerializeField] private float crossFadeDuration = 0.15f;

    private AgentTrigger _trigger;
    private Coroutine _endRoutine;

    public override void InitializeSkill(ISkillModule skillModule)
    {
        base.InitializeSkill(skillModule);
        _trigger = _ownerEnemy.GetModule<AgentTrigger>();
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
        PlayAttackClip();

        if (_trigger != null)
        {
            _trigger.OnAnimationEnd += StopSkill;
            return;
        }

        _endRoutine = StartCoroutine(EndWhenClipFinishes());
    }

    public override void StopSkill()
    {
        if (_endRoutine != null)
        {
            StopCoroutine(_endRoutine);
            _endRoutine = null;
        }

        if (_trigger != null)
            _trigger.OnAnimationEnd -= StopSkill;

        base.StopSkill();
    }

    private void PlayAttackClip()
    {
        if (skillAnimParam == null)
            return;

        if (_renderer != null)
        {
            _renderer.PlayClip(skillAnimParam.HashValue, 0, crossFadeDuration);
            return;
        }

        Animator animator = _ownerEnemy.GetComponentInChildren<Animator>();
        animator?.CrossFadeInFixedTime(skillAnimParam.HashValue, crossFadeDuration, 0, 0f);
    }

    private IEnumerator EndWhenClipFinishes()
    {
        yield return null;

        float wait = 0.5f;
        Animator animator = _ownerEnemy.GetComponentInChildren<Animator>();
        if (animator != null && skillAnimParam != null)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.shortNameHash == skillAnimParam.HashValue)
                wait = Mathf.Max(info.length, 0.1f);
        }

        yield return new WaitForSeconds(wait);

        if (IsUsing)
            StopSkill();
    }
}
