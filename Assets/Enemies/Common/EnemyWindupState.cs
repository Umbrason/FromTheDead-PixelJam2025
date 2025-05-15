using UnityEngine;

[CreateAssetMenu(menuName = "EnemyBehaviourState/Windup")]
public class EnemyWindupState : EnemyBehaviourState
{
    EnemyBehaviourState NextState;
    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    protected override void OnTick()
    {
        if (Owner.Animator.LoopCount > 0)
            Owner.CurrentState = NextState;
    }
}
