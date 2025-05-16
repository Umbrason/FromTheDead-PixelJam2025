using UnityEngine;

public class HitFlash : MonoBehaviour
{
    Cached<HealthPool> cached_healthPool;
    HealthPool HealthPool => cached_healthPool[this];

    void Awake()
    {
        HealthPool.OnModifiedWithSource += OnHealthModified;
    }

    void OnDestroy()
    {
        if (HealthPool) HealthPool.OnModifiedWithSource -= OnHealthModified;
    }

    void OnHealthModified(int change, HealthEvent healthEvent)
    {
        if (healthEvent != null && healthEvent.Amount < 0 && healthEvent.Source != gameObject) HitFlashRenderFeature.Flash(gameObject);
    }
}
