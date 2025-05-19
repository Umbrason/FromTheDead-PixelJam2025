using UnityEngine;

public class BrainProjectile : Projectile
{
    Vector2 direction;
    protected override Vector2 ModifiedDirection => direction;
    protected override float MaxFlightTime { get => EnemyPositionMarker.EnemyCount > 0 ? base.MaxFlightTime : 1f; set => base.MaxFlightTime = value; }
    [SerializeField] float TurnSpeed = 7.5f;
    public override void Shoot(Vector2 direction, Vector2 speedAtLaunch)
    {
        base.Shoot(direction, speedAtLaunch);
        this.direction = direction;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (CurrentState != State.Shooting) return;
        if (EnemyPositionMarker.EnemyCount == 0) return;
        var selfPos = transform.position._xz();
        var deltaFromClosestEnemy = EnemyPositionMarker.ClosestFrom(selfPos) - transform.position._xz();
        var startAngle = -Vector2.SignedAngle(Vector2.up, direction); //why counter clockwise?! who the fuck designed this
        var angleToPlayer = -Vector2.SignedAngle(Vector2.up, deltaFromClosestEnemy.normalized);
        var t = Mathf.Clamp01((FlightTime - .1f) / 1f);
        var angle = Mathf.MoveTowardsAngle(startAngle, angleToPlayer, t * TurnSpeed) * Mathf.PI / 180f;
        direction = new(Mathf.Sin(angle), Mathf.Cos(angle));
    }
}