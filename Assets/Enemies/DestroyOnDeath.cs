using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    Cached<HealthPool> cached_HealthPool;
    HealthPool HealthPool => cached_HealthPool[this];
    private void OnEnable()
    {
        if (HealthPool) HealthPool.OnDepleted += DestoyOnDeath;
    }

    private void OnDisable()
    {
        if (HealthPool) HealthPool.OnDepleted -= DestoyOnDeath;
    }

    void DestoyOnDeath() => Destroy(gameObject);
}
