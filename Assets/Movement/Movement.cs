using UnityEngine;

public class Movement : MonoBehaviour
{
    Cached<VelocityController> Cached_VC;
    VelocityController VC => Cached_VC[this];
    public Vector2 Direction { get; set; }
    [field: SerializeField] public float Speed { get; set; }
    void FixedUpdate()
    {
        VC.AddOverwriteMovement(new(Direction._x0y() * Speed), 0, 0);
    }
}
