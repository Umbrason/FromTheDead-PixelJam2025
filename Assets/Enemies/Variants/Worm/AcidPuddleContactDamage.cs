using UnityEngine;

public class AcidPuddleContactDamage : MonoBehaviour
{
    private static float lastDamageEventTime;
    static float Tickrate = 2f;
    [SerializeField] private int DamageAmount;

    [SerializeField] private AudioClipPlayer SFX;


    void OnTriggerEnter(Collider c) => OnTriggerStay(c);
    void OnTriggerStay(Collider c)
    {
        var hitbox = c.GetComponentInParent<Hitbox>();
        if (!hitbox) return;
        if (Time.time < lastDamageEventTime + 1f / Tickrate) return;
        SFX.Play();
        lastDamageEventTime = Time.time;
        hitbox.RegisterDamageEvent(HealthEvent.Damage((uint)DamageAmount, false, gameObject));
    }
}
