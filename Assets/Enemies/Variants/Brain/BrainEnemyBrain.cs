using System.Collections;
using UnityEngine;

public class BrainEnemyBrain : BaseEnemyBrain
{
    [SerializeField] private BrainEnemySpikeAttack spikeAttack;
    [SerializeField] private ScriptableSpriteAnimation MoveAnimation;
    [SerializeField] private ScriptableSpriteAnimation ChargeAnimation;
    [SerializeField] private ScriptableSpriteAnimation SpawnAnimation;
    [SerializeField] private AudioClipPlayer Stmop;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float spawnBurstCooldown = 6f;
    Cached<Hitbox> cached_hitbox;
    Hitbox Hitbox => cached_hitbox[this];

    protected override IEnumerator FirstThought()
    {
        float lastSpawnTime = 0f;
        bool isOffCooldown() => lastSpawnTime + spawnBurstCooldown < Time.time;
        Vector2 patrolTargetProvider() => EnemyBrainUtils.RandomPatrolPosition(transform.position._xz(), 5f, 10f);
        while (true)
        {
            this.Animator.Current = MoveAnimation;
            yield return EnemyBrainUtils.Patrol(SelfPosition, isOffCooldown, patrolTargetProvider, MoveRoutine, null);
            yield return SpawnBrainAttack();
            lastSpawnTime = Time.time;
        }
    }

    IEnumerator MoveRoutine(Vector2 delta)
    {
        VelocityController.AddOverwriteMovement(new(Vector2.ClampMagnitude(delta, 1)._x0y() * speed), 0, 0);
        yield return new WaitForFixedUpdate();
    }

    IEnumerator SpawnBrainAttack()
    {
        bool interrupted = false;
        void Interrupt(int damageTaken) => interrupted |= -damageTaken > 0;
        HealthPool.OnModified += Interrupt;
        Hitbox.IsCritical = true;
        this.Animator.Current = ChargeAnimation;
        while (this.Animator.LoopCount < 1)
        {
            VelocityController.AddOverwriteMovement(new(Vector2.zero), 0, 0);
            if (interrupted) break;
            yield return new WaitForFixedUpdate();
        }
        HealthPool.OnModified -= Interrupt;
        Hitbox.IsCritical = false;
        if (interrupted) yield break;
        Stmop.Play();
        this.Animator.Current = SpawnAnimation;
        /* var spawnPosition = EnemyBrainUtils.RandomPatrolPosition(EnemyBrainUtils.PlayerPosition(), 1, 1); */
        var instance = Instantiate(spikeAttack, transform.position, Quaternion.identity);
        while (instance != null)
        {
            VelocityController.AddOverwriteMovement(new(Vector2.zero), 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }
}
