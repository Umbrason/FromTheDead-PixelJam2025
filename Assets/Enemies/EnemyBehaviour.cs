using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public Cached<SpriteAnimator> cached_Animator = new(Cached<SpriteAnimator>.GetOption.Children);
    public SpriteAnimator Animator => cached_Animator[this];

    private EnemyBehaviourState m_CurrentState;
    public EnemyBehaviourState CurrentState
    {
        get => m_CurrentState;
        set
        {
            value = Instantiate(value);
            m_CurrentState?.Exit(this);
            m_CurrentState = value;
            value.Enter(this);
            Animator.Current = value.animation;
        }
    }

    void FixedUpdate()
    {
        CurrentState?.Tick();
    }

}