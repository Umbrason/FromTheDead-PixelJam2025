using System.Collections;
using UnityEngine;

public abstract class BaseEnemyBrain : MonoBehaviour
{
    Cached<SpriteAnimator> cached_Animator = new(Cached<SpriteAnimator>.GetOption.Children);
    protected SpriteAnimator Animator => cached_Animator[this];

    Cached<HealthPool> cached_HealthPool;
    protected HealthPool HealthPool => cached_HealthPool[this];

    Cached<VelocityController> cached_VelocityController;
    protected VelocityController VelocityController => cached_VelocityController[this];

    Coroutine CurrentThoughts;
    protected abstract IEnumerator FirstThought();
    protected Vector2 SelfPosition() => transform.position._xz();

    void Start() => CurrentThoughts = StartCoroutine(FirstThought());

    void OnEnable()
    {
        HealthPool.OnDepleted += Die;
    }

    void OnDisable()
    {
        HealthPool.OnDepleted -= Die;
    }

    void Die()
    {
        StopCoroutine(CurrentThoughts);
    }

    protected virtual void OnDied() { }
}