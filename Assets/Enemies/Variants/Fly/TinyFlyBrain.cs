using System.Collections;
using UnityEngine;

public class TinyFlyBrain : BaseEnemyBrain
{
    public BigFlyBrain Creator;
    [SerializeField] private float playerDetectionRange = 10f;
    [SerializeField] private float playerChaseSpeed = 5f;
    [SerializeField] private float circlingSpeed = 4f;
    [SerializeField] private float circlingRadius = 1.5f;
    protected override IEnumerator FirstThought()
    {
        var deltaFromCreator = SelfPosition() - Creator.SelfPosition();
        var startAngle = -Vector2.SignedAngle(Vector2.up, deltaFromCreator.normalized) / 180f * Mathf.PI; //why counter clockwise?! who the fuck designed this
        yield return EnemyBrainUtils.BehaviourWithExitCondition(CircleCreator(startAngle), PlayerInRange);
        yield return RushPlayer();
    }
    bool PlayerInRange() => EnemyBrainUtils.PlayerDistance(SelfPosition()) <= playerDetectionRange;
    IEnumerator CircleCreator(float startAngle)
    {
        var direction = Random.value > .5 ? 1 : -1;
        var segments = Mathf.PI / Mathf.Asin(circlingSpeed * Time.fixedDeltaTime / circlingRadius);
        segments = Mathf.RoundToInt(segments);
        Vector2 calcTargetPos(float angle) => new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * circlingRadius + Creator.SelfPosition();
        while (true)
        {
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
    private IEnumerator RushPlayer()
    {
        Vector2 noiseOffset = Random.insideUnitCircle * 1000;
        while (true)
        {
            var noisePos = SelfPosition() / 3f + noiseOffset;
            var offset = new Vector2(Mathf.PerlinNoise(noisePos.x, noisePos.y), Mathf.PerlinNoise(noisePos.x - 512.754312f, noisePos.y + 74.1236f)) * 2 - Vector2.one;
            var deltaFromPlayer = (EnemyBrainUtils.PlayerPosition() - SelfPosition()).normalized + offset * .5f;
            VelocityController.AddOverwriteMovement(new(deltaFromPlayer.normalized._x0y() * playerChaseSpeed), 0, 0);
            yield return new WaitForFixedUpdate();
        }
    }
}
