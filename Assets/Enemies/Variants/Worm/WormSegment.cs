using UnityEngine;

public class WormSegment : MonoBehaviour
{
    Cached<VelocityController> cached_VelocityController;
    protected VelocityController VelocityController => cached_VelocityController[this];

    private Rigidbody Target;
    [SerializeField] private float FollowDistance = 1f;

    void OnEnable()
    {
        if (Target != null) return;
        var selfSiblingIndex = transform.GetSiblingIndex();
        for (int i = selfSiblingIndex - 1; i > 0; i--)
            if (Target = transform.parent.GetChild(i).GetComponent<Rigidbody>())
                return;
        Target = transform.parent.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!Target) return;
        var targetPos = Target.position._xz();
        var selfPos = transform.position._xz();
        var delta = selfPos - targetPos;
        delta = Vector2.ClampMagnitude(delta, FollowDistance);
        targetPos += delta;
        VelocityController.AddOverwriteMovement(new((targetPos - selfPos)._x0y() / Time.fixedDeltaTime), 0, 0);
        transform.localPosition = transform.localPosition._x0z() + Target.transform.localPosition._0y0() + Vector3.up * -.02f;
    }
}
