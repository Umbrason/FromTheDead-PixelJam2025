using System.Collections;
using UnityEngine;

public class AcidPuddleDespawnAfterTimeOrOwnerDeath : MonoBehaviour
{
    public GameObject Owner;

    [SerializeField] private float Lifetime = 5f;
    private float spawnTime;

    void OnEnable() => spawnTime = Time.time;
    bool despawning;
    void FixedUpdate()
    {
        if (despawning) return;
        if (Time.time - spawnTime > Lifetime || Owner == null)
        {
            despawning = true;
            StartCoroutine(Despawn());
        }
    }

    IEnumerator Despawn()
    {
        var t = 0f;
        while (t < 1)
        {
            transform.localScale = Vector3.one * (1 - (t += Time.deltaTime));
            yield return null;
        }
        Destroy(gameObject);
    }
}
