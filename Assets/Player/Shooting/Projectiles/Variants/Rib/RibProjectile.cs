using UnityEngine;

public class RibProjectile : Projectile
{
    protected override float maxFlightTime { get; } = 2f;
    Vector2 direction;
    protected override Vector2 ModifiedDirection => direction;

    public override void Shoot(Vector2 direction, Vector2 speedAtLaunch)
    {
        base.Shoot(direction, speedAtLaunch);
        this.direction = direction;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (CurrentState != State.Shooting || FlightTime < .2f) return;
        var deltaFromPlayer = EnemyBrainUtils.PlayerPosition() - transform.position._xz();
        if (deltaFromPlayer.sqrMagnitude <= 1) Drop();

        var startAngle = -Vector2.SignedAngle(Vector2.up, direction); //why counter clockwise?! who the fuck designed this
        var angleToPlayer = -Vector2.SignedAngle(Vector2.up, deltaFromPlayer.normalized);

        var t = Mathf.Clamp01((FlightTime - .2f) / 3f);
        var angle = Mathf.MoveTowardsAngle(startAngle, angleToPlayer, t * 30f) * Mathf.PI / 180f;
        direction = new(Mathf.Sin(angle), Mathf.Cos(angle));
    }
}
