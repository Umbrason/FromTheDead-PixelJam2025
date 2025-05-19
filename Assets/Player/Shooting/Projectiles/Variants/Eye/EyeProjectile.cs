using UnityEngine;

public class EyeProjectile : Projectile
{
    [SerializeField] private int maxBounces = 5;
    private int bounces;

    protected override float MaxFlightTime { get => (maxBounces - bounces + 1) * base.MaxFlightTime; set => base.MaxFlightTime = value; }

    private Vector2 currentDirection;
    protected override Vector2 ModifiedDirection => currentDirection;

    public override void Shoot(Vector2 direction, Vector2 speedAtLaunch)
    {
        base.Shoot(direction, speedAtLaunch);
        bounces = maxBounces;
        currentDirection = direction;
    }

    void OnCollisionStay(Collision c)
    {
        var normal = (c.collider.ClosestPoint(transform.position) - transform.position)._xz();
        if (Vector2.Dot(currentDirection, normal) > 0) currentDirection = Vector2.Reflect(currentDirection, normal.normalized);
    }

    protected override void OnHit(Hitbox hitbox, Vector3 point, Vector3 normal, bool isPiercing = false)
    {
        bounces--;
        if (hitbox != null)
        {
            hitbox.RegisterDamageEvent(damageEvent);
            damageEvent = HealthEvent.Damage((uint)-damageEvent.Amount, damageEvent.CanCrit, damageEvent.Source);
        }
        if (bounces <= 0) Drop();
    }
}
