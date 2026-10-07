using CombatSystem;
using ModuleSystem;
using UnityEngine;
using UnityEngine.Events;

public class Agent : ModuleOwner 
{
    public bool IsDead { get; set; }
    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    public EnemyHealthModule Health { get; private set; }
    public ActionDataModule ActionData { get; private set; }

    protected override void InitializeModules()
    {
        base.InitializeModules();
        Health = GetModule<EnemyHealthModule>();
        ActionData = GetModule<ActionDataModule>();
    }
    protected override void AfterInitializeModules()
    {
        base.AfterInitializeModules();
        Health.OnDeath += HandleDeath;
    }

    protected virtual void OnDestroy()
    {
        Health.OnDeath -= HandleDeath;
    }

    protected virtual void HandleDeath()
    {
        IsDead = true;
        OnDeath?.Invoke(); //Agent의 사망을 알림.
    }

    public void ApplyDamage(DamageData damageData)
    {
        if (IsDead) return;
        if (ActionData != null)
        {
            ActionData.HitPoint = damageData.HitPoint;
            ActionData.HitNormal = damageData.HitNormal;
            ActionData.Attacker = damageData.Attacker;
        }

        Health?.ApplyDamage(damageData.DamageAmount);

        OnHit?.Invoke();
    }


}
