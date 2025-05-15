using UnityEngine;

public abstract class EnemyBehaviourState : ScriptableObject
{
    [SerializeField] public ScriptableSpriteAnimation animation;
    protected EnemyBehaviour Owner { get; private set; }
    public void Enter(EnemyBehaviour enemy)
    {
        Owner = enemy;
        OnEnter();
    }
    public void Exit(EnemyBehaviour enemy)
    {
        Owner = null;
        OnExit();
    }

    public void Tick()
    {
        OnTick();
    }

    protected virtual void OnEnter() { }
    protected virtual void OnTick() { }
    protected virtual void OnExit() { }

}
