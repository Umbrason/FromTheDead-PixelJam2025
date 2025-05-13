using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Cached<VelocityController> cached_VelocityController;
    private VelocityController VelocityController => cached_VelocityController[this];

    private Cached<Rigidbody> cached_Rigidbody;
    private Rigidbody Rigidbody => cached_Rigidbody[this];

    [field: SerializeField] protected int BaseDamage { get; private set; } = 1;
    [field: SerializeField] protected float LaunchSpeed { get; private set; } = 5;
    protected Vector2 LaunchDirection { get; private set; }
    private float launchStartTime;
    protected float FlightTime => enabled ? Time.time - launchStartTime : 0;

    protected virtual float ModifiedSpeed => LaunchSpeed;
    protected virtual Vector2 ModifiedDirection => LaunchDirection;

    void Awake() => Drop();

    protected virtual void OnHit(Hitbox hitbox, Vector3 point, Vector3 normal)
    {
        if (hitbox != null) hitbox.RegisterDamageEvent(damageEvent);
        Drop();
    }

    private static readonly List<ContactPoint> contactPoints = new();
    private void OnCollisionEnter(Collision collision)
    {
        var hitbox = collision.collider.GetComponentInParent<Hitbox>();
        if (hitbox != null)
            contactPoints.Clear();
        collision.GetContacts(contactPoints);
        var pos = Vector3.zero;
        var normal = Vector3.zero;
        foreach (var point in contactPoints)
        {
            pos += point.point;
            normal += point.normal;
        }
        pos /= contactPoints.Count;
        normal /= contactPoints.Count;
        OnHit(hitbox, pos, normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        var hitbox = other.GetComponentInParent<Hitbox>();
        var generatedPos = other.ClosestPoint(transform.position);
        var generatedNormal = (transform.position - generatedPos).normalized;
        OnHit(hitbox, generatedPos, generatedNormal);
    }

    protected void Drop()
    {
        transform.SetLayerRecursive(0);
        enabled = false;
        launchStartTime = float.PositiveInfinity;
        Rigidbody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        VelocityController.AddOverwriteMovement(new(ModifiedDirection._x0y().normalized * ModifiedSpeed), 0f, 0);
    }

    private HealthEvent damageEvent;
    public void Shoot(Vector2 direction)
    {
        transform.SetLayerRecursive(LayerMask.NameToLayer("PlayerProjectile"));
        transform.SetParent(null);
        enabled = true;
        Rigidbody.isKinematic = false;
        Rigidbody.linearVelocity = default;
        launchStartTime = Time.time;
        LaunchDirection = direction;
        damageEvent = HealthEvent.Damage((uint)BaseDamage, source: gameObject);

    }
}
