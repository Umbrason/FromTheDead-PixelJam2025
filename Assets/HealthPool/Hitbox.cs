using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [field: SerializeField] public bool IsCritical { get; private set; }
    Cached<HealthPool> cached_HealthPool = new(Cached<HealthPool>.GetOption.Parent);
    HealthPool HealthPool => cached_HealthPool[this];

    public event Action<HealthEvent> OnDamageEventRegistered;
    public void RegisterDamageEvent(HealthEvent damageEvent)
    {
        HealthPool.RegisterHealthEvent(damageEvent, (IsCritical && damageEvent.CanCrit ? 2 : 1) * (enabled ? 1 : 0));
        OnDamageEventRegistered?.Invoke(damageEvent);
    }
}