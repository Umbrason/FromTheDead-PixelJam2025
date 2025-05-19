using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform Target;

    [SerializeField] private Spring.Config config;
    private Vector2Spring PositionSpring;


    Vector2 lastPosition;
    Vector2 currentPosition;

    void FixedUpdate()
    {
        if (!Target) return;
        lastPosition = currentPosition;
        currentPosition = Target.position._xz();
    }

    void Awake()
    {
        PositionSpring = new(config);
        PositionSpring.RestingPos = PositionSpring.Position = transform.position._xz();
    }


    void LateUpdate()
    {
        var t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;

        PositionSpring.RestingPos = Vector2.Lerp(lastPosition, currentPosition, t);
        PositionSpring.Step(Time.deltaTime);
        transform.position = PositionSpring.Position._x0y();
    }
}
