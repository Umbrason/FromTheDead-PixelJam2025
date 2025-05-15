using UnityEngine;

[CreateAssetMenu(menuName = "EnemyBehaviourState/Patrol")]
public class EnemyPatrolState : EnemyBehaviourState
{
    public Cached<VelocityController> cached_VelocityController = new(Cached<VelocityController>.GetOption.Children);
    public VelocityController VelocityController => cached_VelocityController[Owner];

    [SerializeField] float movementSpeed;

    private Vector2 targetPosition;
    protected override void OnTick()
    {
        var delta = targetPosition - VelocityController.transform.position._xz();
        VelocityController.AddOverwriteMovement(new(delta.normalized._x0y() * movementSpeed), 0, 0);
    }
}
