using System.Collections;
using UnityEngine;

public class HandBrain : BaseEnemyBrain
{
    [SerializeField] private HandBoneAttack boneAttackTemplate;
    [SerializeField] private ScriptableSpriteAnimation MoveAnimation;
    [SerializeField] private ScriptableSpriteAnimation SpawnAnimation;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float spawnBurstCooldown = 6f;
    [SerializeField] private int spawnBurstSize = 3;
    [SerializeField] AudioClipPlayer launchSFX;

    protected override IEnumerator FirstThought()
    {
        float lastSpawnTime = 0f;
        bool isOffCooldown() => lastSpawnTime + spawnBurstCooldown < Time.time;
        Vector2 patrolTargetProvider() => EnemyBrainUtils.RandomPatrolPosition(transform.position._xz(), 5f, 10f);
        while (true)
        {
            this.Animator.Current = MoveAnimation;
            yield return EnemyBrainUtils.Patrol(SelfPosition, isOffCooldown, patrolTargetProvider, MoveRoutine, null);
            yield return SpawnBoneAttacks();
            lastSpawnTime = Time.time;
        }
    }

    IEnumerator MoveRoutine(Vector2 delta)
    {
        VelocityController.AddOverwriteMovement(new(Vector2.ClampMagnitude(delta, 1)._x0y() * speed), 0, 0);
        yield return new WaitForFixedUpdate();
    }

    IEnumerator SpawnBoneAttacks()
    {
        for (int i = 0; i < spawnBurstSize; i++)
        {
            this.Animator.Current = SpawnAnimation;
            launchSFX.Play();
            while (this.Animator.LoopCount < 1)
            {
                VelocityController.AddOverwriteMovement(new(Vector2.zero), 0, 0);
                yield return new WaitForFixedUpdate();
            }
            for (int j = 0; j < 2; j++)
            {
                var spawnPosition = EnemyBrainUtils.RandomPatrolPosition(EnemyBrainUtils.PlayerPosition(), 2f, attackRadius);
                Instantiate(boneAttackTemplate, spawnPosition._x0y(), Quaternion.identity);
            }
        }
    }
}
