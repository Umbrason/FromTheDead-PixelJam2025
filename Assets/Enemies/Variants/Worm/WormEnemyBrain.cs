using System;
using System.Collections;
using UnityEngine;

public class WormEnemyBrain : BaseEnemyBrain
{
    [SerializeField] private ScriptableSpriteAnimation MoveAnimation;
    [SerializeField] private float triggerCirclingBehaviourRadius = 15;
    [SerializeField] private float circleBehaviourRadius = 5;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float circlingSpeed = 10f;

    Cached<VelocityController> cached_VelocityController;
    VelocityController VelocityController => cached_VelocityController[this];

    protected override IEnumerator FirstThought()
    {
        this.Animator.Current = MoveAnimation;
        while (true)
        {
            bool isPlayerInRange() => EnemyBrainUtils.PlayerDistance(transform.position._xz()) < triggerCirclingBehaviourRadius;
            Vector2 patrolTargetProvider() => EnemyBrainUtils.RandomPatrolPosition(transform.position._xz());
            yield return EnemyBrainUtils.Patrol(SelfPosition, isPlayerInRange, patrolTargetProvider, MovePatrol, null);

            //player must be in range to reach this point
            var deltaToPlayer = EnemyBrainUtils.PlayerPosition() - SelfPosition();
            var angleToPlayer = Vector2.SignedAngle(deltaToPlayer, Vector2.up);
            yield return CirclePlayer(angleToPlayer, circleBehaviourRadius);
        }
    }

    private IEnumerator MovePatrol(Vector2 deltaToTarget)
    {
        VelocityController.AddOverwriteMovement(new(deltaToTarget.normalized, speed), 0, 0);
        yield return null;
    }

    private IEnumerator MoveCircling(Vector2 deltaToTarget)
    {
        VelocityController.AddOverwriteMovement(new(deltaToTarget.normalized, circlingSpeed), 0, 0);
        yield return null;
    }

    private IEnumerator CirclePlayer(float startAngle, float radius = 5f)
    {
        var segments = Mathf.Asin(speed / radius) / Mathf.PI;
        segments = Mathf.RoundToInt(segments);
        Vector2 calcTargetPos(float angle) => new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius + EnemyBrainUtils.PlayerPosition();
        for (int i = 0; i < segments; i++)
        {
            var targetAngle = startAngle + Mathf.PI * 2 / segments;
            var targetPos = calcTargetPos(targetAngle);
            Vector2 delta;
            while ((delta = targetPos - SelfPosition()).sqrMagnitude > .01f)
            {
                targetPos = calcTargetPos(targetAngle);
                yield return MoveCircling(delta);
            }
        }
    }
}