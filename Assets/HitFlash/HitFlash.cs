using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    Cached<HealthPool> cached_healthPool;
    HealthPool HealthPool => cached_healthPool[this];

    void Awake()
    {
        HealthPool.OnModified += OnHealthModified;
    }

    void OnDestroy()
    {
        if (HealthPool) HealthPool.OnModified -= OnHealthModified;
    }

    void OnHealthModified(int change)
    {
        if (change < 0) HitFlashRenderFeature.Flash(gameObject);
    }
}
