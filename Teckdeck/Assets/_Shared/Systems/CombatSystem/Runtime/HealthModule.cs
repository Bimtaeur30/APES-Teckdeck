using ModuleSystem;
using System;
using UnityEngine;

namespace CombatSystem
{
    public class HealthModule : MonoBehaviour, IModule, IHealthModule
    {
        //[SerializeField] private StatSO vitalStat;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private float baseMaxHealth;

        public event Action<float, float> OnHealthChanged;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        
        private ModuleOwner _owner;
        //private IStatModule _statModule;

        public event Action OnDeath;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            currentHealth = maxHealth; 
            //_statModule = owner.GetModule<IStatModule>();
        }
        
        private void Start()
        {
            RaiseHealthChangeEvent();
        }

        private void OnDestroy()
        {
            //_statModule?.UnSubscribeStat(vitalStat.AssetIndex, HandleVitalChange);
        }

        private void HandleVitalChange(/*StatSO stat*/ float currentValue, float prevValue)
        {
            float beforeMaxHealth = maxHealth;
            maxHealth = baseMaxHealth + 5 * currentValue;
            float delta = maxHealth - beforeMaxHealth;
            currentHealth = Mathf.Clamp(currentHealth + delta, 1, maxHealth);
            RaiseHealthChangeEvent();
        }
        [ContextMenu("asdf")]
        public void ApplyDamage() => ApplyDamage(50);

        public void ApplyDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
            RaiseHealthChangeEvent();
            if (currentHealth <= 0f)
                OnDeath?.Invoke();
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || maxHealth <= 0f) return;
            float next = Mathf.Min(maxHealth, currentHealth + amount);
            if (Mathf.Approximately(next, currentHealth)) return;
            currentHealth = next;
            RaiseHealthChangeEvent();
        }

        public void SetHealth(float current, float max)
        {
            maxHealth = Mathf.Max(1f, max);
            currentHealth = Mathf.Clamp(current, 0f, maxHealth);
            RaiseHealthChangeEvent();
        }

        private void RaiseHealthChangeEvent()=> OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
    }
}