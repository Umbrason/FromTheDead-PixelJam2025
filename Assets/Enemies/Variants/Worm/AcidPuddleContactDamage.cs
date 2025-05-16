using UnityEngine;

public class AcidPuddleContactDamage : MonoBehaviour
{
    private static float lastDamageEventTime;
    static float Tickrate = 2f;
    [SerializeField] private int DamageAmount;
    void OnTriggerStay(Collider c)
    {
        var hitbox = c.GetComponentInParent<Hitbox>();
        if (!hitbox) return;
        if (Time.time < lastDamageEventTime + 1f / Tickrate) return;
        lastDamageEventTime = Time.time;
        hitbox.RegisterDamageEvent(HealthEvent.Damage((uint)DamageAmount, false, gameObject));
    }
}
