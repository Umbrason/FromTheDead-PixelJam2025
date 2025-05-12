using UnityEngine;

public class Dash : Ability
{
    Cached<VelocityController> Cached_VC;
    VelocityController VC => Cached_VC[this];

    Cached<Movement> Cached_Movement;
    Movement Movement => Cached_Movement[this];
    [field: SerializeField] public float Duration { get; set; }
    [field: SerializeField] public float Range { get; set; }
    public float Speed => Range / Duration;
    [SerializeField] private float DashCooldown;

    public override float CooldownDuration => DashCooldown;
    public override bool CanUse => Movement.Direction.sqrMagnitude >= .001f;

    public override void OnPress()
    {
        VC.AddOverwriteMovement(new(Movement.Direction._x0y().normalized * Speed), Duration, 5);
    }

    public override void OnUsing()
    {
        if (CurrentUseTime >= Duration) CompleteUse();
    }
}
