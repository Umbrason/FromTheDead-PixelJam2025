using UnityEngine;

public class IntestineProjectile : Projectile
{
    Cached<SpawnPuddle> cached_puddleSpawner;
    SpawnPuddle PuddleSpawner => cached_puddleSpawner[this];
    public override void Shoot(Vector2 direction, Vector2 speedAtLaunch)
    {
        base.Shoot(direction, speedAtLaunch);
        PuddleSpawner.enabled = true;
    }

    protected override void Drop()
    {
        base.Drop();
        PuddleSpawner.enabled = false;
    }


}
