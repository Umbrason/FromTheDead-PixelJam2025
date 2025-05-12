using System;
using UnityEngine;

public class HealthEvent
{
    public Guid GUID { get; }
    public int Amount { get; }
    public bool CanCrit { get; }
    public GameObject Source { get; }

    public event Action OnReleased;
    public void Release() => OnReleased?.Invoke();

    public event Action<HitReport> OnHitHealthPool;
    public void ReportHit(HealthPool pool, int changeAmount, bool depleted) => OnHitHealthPool?.Invoke(new(pool, changeAmount, depleted));
    public readonly struct HitReport
    {
        public readonly HealthPool pool;
        public readonly int changeAmount;
        public readonly bool depleted;

        public HitReport(HealthPool pool, int changeAmount, bool depleted)
        {
            this.pool = pool;
            this.changeAmount = changeAmount;
            this.depleted = depleted;
        }
    }

    private HealthEvent(int amount, bool canCrit = true, GameObject source = null)
    {
        this.GUID = Guid.NewGuid();
        this.Amount = amount;
        this.CanCrit = canCrit;
        this.Source = source;
    }
    public static HealthEvent Damage(uint amount, bool canCrit = true, GameObject source = null) => new(-(int)amount, canCrit, source);
    public static HealthEvent Heal(uint amount, bool canCrit = true, GameObject source = null) => new((int)amount, canCrit, source);
}