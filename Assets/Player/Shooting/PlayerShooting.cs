using UnityEngine;

public class PlayerShooting : Ability
{
    public Vector2 Direction { get; set; }
    Cached<PlayerAmmunition> cached_PlayerAmmunition;
    PlayerAmmunition PlayerAmmunition => cached_PlayerAmmunition[this];

    public override bool CanUse => Direction.sqrMagnitude > .01f && PlayerAmmunition.Next != null;

    public override void OnPress()
    {
        var projectile = PlayerAmmunition.RemoveProjectile(PlayerAmmunition.Next);
        projectile.Shoot(Direction);
        CompleteUse();
    }
}