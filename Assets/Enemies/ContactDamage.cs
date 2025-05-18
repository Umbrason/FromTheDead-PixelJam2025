using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [Tooltip("Damage events per second")][SerializeField] private float tickRate;
    [SerializeField] private int damage;
    [SerializeField] private bool destroyAfter;
    private float lastTick;
    private readonly Dictionary<Hitbox, int> activeCollisions = new();
    void FixedUpdate()
    {
        if (lastTick + (1f / tickRate) >= Time.time) return;
        lastTick = Time.time;
        var dmgEvent = HealthEvent.Damage((uint)damage, source: gameObject);
        dmgEvent.OnHitHealthPool += (report) =>
        {
            if (report.changeAmount != 0 && destroyAfter && gameObject != null) Destroy(gameObject);
        };
        foreach (var hitbox in activeCollisions.Keys)
        {
            hitbox.RegisterDamageEvent(dmgEvent);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        var hitbox = collision.collider.GetComponentInParent<Hitbox>();
        if (!hitbox) return;
        if (activeCollisions.ContainsKey(hitbox)) activeCollisions[hitbox]++;
        else activeCollisions.Add(hitbox, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        var hitbox = other.GetComponentInParent<Hitbox>();
        if (!hitbox) return;
        if (activeCollisions.ContainsKey(hitbox)) activeCollisions[hitbox]++;
        else activeCollisions.Add(hitbox, 1);
    }

    private void OnCollisionExit(Collision collision)
    {
        var hitbox = collision.collider.GetComponentInParent<Hitbox>();
        if (!hitbox) return;
        if (!activeCollisions.ContainsKey(hitbox)) return;
        activeCollisions[hitbox]--;
        if (activeCollisions[hitbox] == 0) activeCollisions.Remove(hitbox);
    }

    private void OnTriggerExit(Collider other)
    {
        var hitbox = other.GetComponentInParent<Hitbox>();
        if (!hitbox) return;
        if (!activeCollisions.ContainsKey(hitbox)) return;
        activeCollisions[hitbox]--;
        if (activeCollisions[hitbox] == 0) activeCollisions.Remove(hitbox);
    }
}
