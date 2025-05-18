using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BigFlyBrain : BaseEnemyBrain
{
    [SerializeField] private TinyFlyBrain tinyFlyTemplate;
    [SerializeField] private ScriptableSpriteAnimation MoveAnimation;
    [SerializeField] private ScriptableSpriteAnimation SpawnAnimation;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float spawnBurstCooldown = 4f;
    [SerializeField] private int spawnBurstSize = 3;
    [SerializeField] private int maxOffspringCount = 5;
    readonly List<TinyFlyBrain> AliveOffspring = new();

    Vector2 movementDirection;
    protected override IEnumerator FirstThought()
    {
        var startDirAngle = Random.value * Mathf.PI * 2;
        movementDirection = new Vector2(Mathf.Sin(startDirAngle), Mathf.Cos(startDirAngle));
        this.Animator.Current = MoveAnimation;
        yield return EnemyBrainUtils.ParallelBehaviour(MoveRoutine(), SpawnTinyFlyBurst());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            VelocityController.AddOverwriteMovement(new(Vector2.ClampMagnitude(movementDirection, 1)._x0y() * speed), 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator SpawnTinyFlyBurst()
    {
        var lastSpawnTime = -spawnBurstCooldown;
        while (true)
        {
            CheckOffspring();
            var openSlots = maxOffspringCount - AliveOffspring.Count;
            var spawnCapReached = openSlots <= 0;
            var onCooldown = Time.time < lastSpawnTime + spawnBurstCooldown;
            if (!(spawnCapReached || onCooldown))
            {
                lastSpawnTime = Time.time;
                var fliesToSpawn = Mathf.Min(spawnBurstSize, openSlots);
                for (int i = 0; i < fliesToSpawn; i++)
                {
                    this.Animator.Current = SpawnAnimation;
                    yield return new WaitUntil(() => this.Animator.LoopCount >= 1);
                    var spawnAngle = (i + Random.value) / fliesToSpawn * Mathf.PI * 2;
                    var spawnDirection = new Vector2(Mathf.Sin(spawnAngle), Mathf.Cos(spawnAngle));
                    var newFly = Instantiate(tinyFlyTemplate, transform.position + spawnDirection._x0y() * .25f, Quaternion.identity);
                    newFly.Creator = this;
                    AliveOffspring.Add(newFly);
                }
                this.Animator.Current = MoveAnimation;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void CheckOffspring()
    {
        var deadOffspring = AliveOffspring.Where(offspring => offspring == null).ToArray();
        foreach (var deadFly in deadOffspring)
            AliveOffspring.Remove(deadFly);
    }

    void OnCollisionStay(Collision c)
    {
        if (c.collider.gameObject.layer != 0) return; //not environment
        var normal = (c.collider.ClosestPoint(transform.position) - transform.position)._xz();
        if (Vector2.Dot(movementDirection, normal) > 0) movementDirection = Vector2.Reflect(movementDirection, normal.normalized);
    }
}
