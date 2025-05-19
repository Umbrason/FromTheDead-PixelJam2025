using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    Cached<HealthPool> cached_HealthPool;
    HealthPool HealthPool => cached_HealthPool[this];
    private void Start()
    {
        if (HealthPool) HealthPool.OnDepleted += DestoyOnDeath;
    }

    private void OnDestroy()
    {
        if (HealthPool) HealthPool.OnDepleted -= DestoyOnDeath;
    }

    void DestoyOnDeath() => Destroy(gameObject);
}
