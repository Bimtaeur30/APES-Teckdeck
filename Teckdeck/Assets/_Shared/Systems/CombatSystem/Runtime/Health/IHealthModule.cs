using System;
using UnityEngine;

public interface IHealthModule
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
}
