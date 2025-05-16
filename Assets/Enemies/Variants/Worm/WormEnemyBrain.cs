using System;
using System.Collections;
using UnityEngine;

public class WormEnemyBrain : BaseEnemyBrain
{
    [SerializeField] private ScriptableSpriteAnimation MoveAnimation;
    [SerializeField] private float triggerCirclingBehaviourRadius = 10;
    [SerializeField] private float circleBehaviourRadius = 5;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float circlingSpeed = 10f;
    [SerializeField] private float circlingCooldown = 4f;

    protected override IEnumerator FirstThought()
    {
        this.Animator.Current = MoveAnimation;
        var lastCirclingTime = Time.time;
        while (true)
        {
            bool isPlayerInRange() => (EnemyBrainUtils.PlayerDistance(transform.position._xz()) < triggerCirclingBehaviourRadius) && (Time.time - lastCirclingTime > circlingCooldown);
            Vector2 patrolTargetProvider() => EnemyBrainUtils.RandomPatrolPosition(transform.position._xz(), 5f, 10f);
            yield return EnemyBrainUtils.Patrol(SelfPosition, isPlayerInRange, patrolTargetProvider, MoveLinear, null);

            //player must be in trigger range to reach this point

            var deltaFromPlayer = default(Vector2);
            do
            {
                deltaFromPlayer = SelfPosition() - EnemyBrainUtils.PlayerPosition() ;
                yield return MoveLinear(-deltaFromPlayer);
            }
            while (deltaFromPlayer.sqrMagnitude >= circleBehaviourRadius * circleBehaviourRadius);
            //worm should be at the circle radius
            var startAngle = -Vector2.SignedAngle(Vector2.up, deltaFromPlayer.normalized) / 180f * Mathf.PI; //why counter clockwise?! who the fuck designed this
            yield return CirclePlayer(startAngle, circleBehaviourRadius);
            lastCirclingTime = Time.time;
        }
    }

    private IEnumerator MoveLinear(Vector2 deltaToTarget)
    {
        VelocityController.AddOverwriteMovement(new(Vector2.ClampMagnitude(deltaToTarget, 1)._x0y() * speed), 0, 0);
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator CirclePlayer(float startAngle, float radius = 5f)
    {
        /*
            l = sin(PI/n) * radius
            l / radius = sin(PI/n)
            asin(l/radius) = PI / n
        */
        var direction = UnityEngine.Random.value > .5 ? 1 : -1;
        var segments = Mathf.PI / Mathf.Asin(circlingSpeed * Time.fixedDeltaTime / radius);
        segments = Mathf.RoundToInt(segments);
        Vector2 calcTargetPos(float angle) => new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius + EnemyBrainUtils.PlayerPosition();
        for (int i = 0; i < segments; i++)
        {
            var targetAngle = startAngle + i * Mathf.PI * 2 / segments * direction;
            var targetPos = calcTargetPos(targetAngle);
            var delta = targetPos - SelfPosition();
            VelocityController.AddOverwriteMovement(new(delta._x0y() / Time.fixedDeltaTime), 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }
}