using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyBrainUtils
{
    public static float PlayerDistance(Vector2 pos) => (PlayerPosition() - pos).magnitude;
    public static Vector2 PlayerPosition() => PlayerPositionMarker.CurrentPosition;
    private static readonly Collider[] colliders = new Collider[1];
    public static Vector2 RandomPatrolPosition(Vector2 origin, float minRange = 5f, float maxRange = 50f)
    {
        for (int i = 0; i < 100; i++)
        {
            var alpha = UnityEngine.Random.value * 3.141f * 2f;
            var r = Mathf.Sqrt(UnityEngine.Random.value) * (maxRange - minRange) + minRange;
            var pos = origin + new Vector2(Mathf.Sin(alpha), Mathf.Cos(alpha)) * r;
            if (Physics.OverlapSphereNonAlloc(pos._x0y(), 2f, colliders) == 0)
                return pos;
        }
        return origin;
    }

    public static IEnumerator Patrol(Func<Vector2> currentPosition, Func<bool> exitCondition, Func<Vector2> targetProvider, Func<Vector2, IEnumerator> Move, Action OnTargetReached)
    {
        while (!exitCondition())
        {
            var targetPos = targetProvider.Invoke();
            Vector2 delta;
            Vector2 currentPos = currentPosition.Invoke();
            while ((delta = targetPos - currentPos).sqrMagnitude > .01f)
            {
                if (exitCondition()) yield break;
                yield return Move.Invoke(delta);
                currentPos = currentPosition.Invoke();
                Debug.DrawLine(currentPos._x0y() + delta._x0y(), currentPos._x0y(), Color.blue, 0f);
            }
            OnTargetReached?.Invoke();
        }
    }

    public static IEnumerator ParallelBehaviour(params IEnumerator[] behaviours)
    {
        var unfinishedBehaviours = behaviours.ToList();
        Dictionary<IEnumerator, Stack<IEnumerator>> subroutineStack = new();
        foreach (var b in unfinishedBehaviours)
        {
            subroutineStack[b] = new();
            subroutineStack[b].Push(b);
        }
        while (unfinishedBehaviours.Count > 0)
        {
            var finishedBehaviours = new List<IEnumerator>();
            foreach (var b in unfinishedBehaviours)
            {
                var routine = subroutineStack[b].Peek();
                if (routine.MoveNext() == false)
                    subroutineStack[b].Pop();
                else if (routine.Current is IEnumerator e) subroutineStack[b].Push(e);
                if (subroutineStack[b].Count == 0) finishedBehaviours.Add(b);
            }
            foreach (var finishedBehaviour in finishedBehaviours) unfinishedBehaviours.Remove(finishedBehaviour);
            yield return null;
        }
    }

    public static IEnumerator BehaviourWithExitCondition(IEnumerator baseBehaviour, Func<bool> exitCondition)
    {
        Stack<IEnumerator> subroutineStack = new();
        subroutineStack.Push(baseBehaviour);
        do
        {
            var routine = subroutineStack.Peek();
            if (routine.MoveNext() == false) subroutineStack.Pop();
            else if (routine.Current is IEnumerator e) subroutineStack.Push(e);
            if (subroutineStack.Count == 0) break;
            yield return routine;
        }
        while (!(exitCondition?.Invoke() ?? true));
    }
}