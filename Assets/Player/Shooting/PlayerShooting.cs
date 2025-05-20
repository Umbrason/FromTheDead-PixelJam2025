using UnityEngine;

public class PlayerShooting : Ability
{
    public enum AimMode
    {
        AbsoluteTarget, RelativeDirection
    }
    public Vector2 AbsoluteTarget { get; set; }
    public Vector2 RelativeDirection { get; set; }
    public AimMode CurrentAimMode { get; set; }
    Vector2 AbsoluteTargetWorldPosition => Camera.main.ViewportPointToRay(AbsoluteTarget).origin._xz();
    public Vector2 LaunchDirection => CurrentAimMode switch
    {
        AimMode.AbsoluteTarget => AbsoluteTargetWorldPosition - transform.position._xz(),
        AimMode.RelativeDirection => RelativeDirection,
        _ => default
    };

    Cached<PlayerAmmunition> cached_PlayerAmmunition;
    PlayerAmmunition PlayerAmmunition => cached_PlayerAmmunition[this];

    Cached<Rigidbody> cached_Rigidbody;
    Rigidbody Rigidbody => cached_Rigidbody[this];

    public override bool CanUse => LaunchDirection.sqrMagnitude > .01f && PlayerAmmunition.Next != null;
    public override float CooldownDuration => .5f;
    [SerializeField] AudioClipPlayer ShootSFX;
    public override void OnPress()
    {
        var projectile = PlayerAmmunition.RemoveProjectile(PlayerAmmunition.Next);
        projectile.transform.position = transform.position;
        projectile.Shoot(LaunchDirection.normalized, Rigidbody.linearVelocity._xz());
        ShootSFX.Play();
        CompleteUse();
    }

    [Header("Aim Indicator")]
    [SerializeField] GameObject AbsolutePositionAimIndicator;
    [SerializeField] GameObject RelativeDirectionAimIndicator;
    [SerializeField] float RelativeDirectionAimIndicatorRange = 5f;
    void LateUpdate()
    {
        AbsolutePositionAimIndicator.SetActive(CurrentAimMode == AimMode.AbsoluteTarget && CanUse);
        RelativeDirectionAimIndicator.SetActive(CurrentAimMode == AimMode.RelativeDirection && CanUse);
        switch (CurrentAimMode)
        {
            case AimMode.AbsoluteTarget:
                AbsolutePositionAimIndicator.transform.position = AbsoluteTargetWorldPosition._x0y();
                AbsolutePositionAimIndicator.transform.localPosition = AbsolutePositionAimIndicator.transform.localPosition._x0z();
                break;
            case AimMode.RelativeDirection:
                var childCount = RelativeDirectionAimIndicator.transform.childCount;
                var minRadius = 2.5f;
                for (int i = 0; i < childCount; i++)
                {
                    var child = RelativeDirectionAimIndicator.transform.GetChild(i);
                    var mag = RelativeDirection.magnitude;
                    child.transform.localPosition = RelativeDirection._x0y().normalized * (mag * (RelativeDirectionAimIndicatorRange - minRadius) * i / (float)childCount + minRadius);
                }
                break;
        }
    }
}