using System;
using System.Collections;
using UnityEngine;

public static class EnemyBrainUtils
{
    public static float PlayerDistance(Vector2 pos) => (PlayerPosition() - pos).magnitude;
    public static Vector2 PlayerPosition() => throw new NotImplementedException();
    public static Vector2 RandomPatrolPosition(Vector2 origin, float minRange = 5f, float maxRange = 50f)
    {
        for (int i = 0; i < 100; i++)
        {
            var alpha = UnityEngine.Random.value * 3.141f * 2f;
            var r = UnityEngine.Random.value * (maxRange - minRange) + minRange;
            var pos = origin + new Vector2(Mathf.Sin(alpha), Mathf.Cos(alpha)) * r;
            if (Physics.OverlapSphere(pos._x0y(), .5f).Length == 0)
                return pos;
        }
        return origin;
    }

    public static IEnumerator Patrol(Func<Vector2> currentPosition, Func<bool> exitCondition, Func<Vector2> targetProvider, Func<Vector2, IEnumerator> Move, Action OnTargetReached)
    {
        while (!exitCondition())
        {
            var targetPos = targetProvider.Invoke();
            var delta = targetPos - currentPosition.Invoke();
            while (delta.sqrMagnitude > .01f)
            {
                if (exitCondition()) yield break;
                yield return Move.Invoke(delta);
            }
            OnTargetReached.Invoke();
        }
    }
}