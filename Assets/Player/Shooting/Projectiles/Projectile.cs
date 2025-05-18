using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [field: SerializeField] public Sprite UIIcon { get; private set; }
    private Cached<VelocityController> cached_VelocityController;
    private VelocityController VelocityController => cached_VelocityController[this];

    private Cached<Rigidbody> cached_Rigidbody;
    public Rigidbody Rigidbody => cached_Rigidbody[this];

    private Cached<Collider> cached_Collider = new(Cached<Collider>.GetOption.Children);
    public Collider Collider => cached_Collider[this];

    [field: SerializeField] protected int BaseDamage { get; private set; } = 1;
    [field: SerializeField] protected float LaunchSpeed { get; private set; } = 5;
    protected Vector2 LaunchDirection { get; private set; }

    public State CurrentState { get; private set; } = State.Shooting;
    public enum State
    {
        Dropped,
        Shooting,
        Collected
    }

    [SerializeField] GameObject DroppedIndicator;
    [SerializeField] GameObject Shadow;
    [SerializeField] GameObject Visual;

    private Vector2 SpeedAtLaunch;
    private float launchStartTime;
    protected float FlightTime => CurrentState == State.Shooting ? Time.time - launchStartTime : 0;

    protected virtual float ModifiedSpeed => LaunchSpeed;
    protected virtual Vector2 ModifiedDirection => LaunchDirection;

    void Awake() => Drop();
    protected virtual void OnHit(Hitbox hitbox, Vector3 point, Vector3 normal, bool isPiercing = false)
    {
        if (hitbox != null) hitbox.RegisterDamageEvent(damageEvent);
        if (!isPiercing) Drop();
    }

    [SerializeField] private int minFlightHeightInPixels = 6;
    [SerializeField] private int currentFlightHeightInPixels;
    [SerializeField] private int maxflightHeightInPixels = 10;
    private float currentFlightHeight => currentFlightHeightInPixels / 16f;
    [SerializeField] private float dropAnimationDuration = .25f;
    [SerializeField] GameObject DropImpactVFX;
    private IEnumerator DropAnimation()
    {
        DroppedIndicator.SetActive(true);
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / dropAnimationDuration;
            t = Mathf.Min(1, t);
            Visual.transform.localPosition = (1 + Mathf.Sqrt(t) - t * t * 2) * currentFlightHeight * 1.5f * Vector3.forward;
            DroppedIndicator.transform.localScale = Vector3.one * t;
            yield return null;
        }
        DropImpactVFX.SetActive(CurrentState == State.Dropped);
        DroppedIndicator.transform.localScale = Vector3.one;
        Visual.transform.localPosition = default;
    }

    private static readonly List<ContactPoint> contactPoints = new();
    private void OnCollisionEnter(Collision collision)
    {
        if (damageEvent == null)
        {
            if (owner != null) owner.gameObject.SendMessageUpwards(nameof(OnCollisionEnter), collision);
            return;
        }
        var hitbox = collision.collider.GetComponentInParent<Hitbox>();
        if (hitbox != null)
            contactPoints.Clear();
        collision.GetContacts(contactPoints);
        var pos = Vector3.zero;
        var normal = Vector3.zero;
        foreach (var point in contactPoints)
        {
            pos += point.point;
            normal += point.normal;
        }
        pos /= contactPoints.Count;
        normal /= contactPoints.Count;
        OnHit(hitbox, pos, normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageEvent == null)
        {
            if (owner != null) owner.gameObject.SendMessageUpwards(nameof(OnTriggerEnter), other);
            return;
        }
        /* if (other.isTrigger) return; //only meant for piercing projectiles hitting regular hitboxes */
        var hitbox = other.GetComponentInParent<Hitbox>();
        var generatedPos = other.ClosestPoint(transform.position);
        var generatedNormal = (transform.position - generatedPos).normalized;
        OnHit(hitbox, generatedPos, generatedNormal, true);
    }

    PlayerAmmunition owner;
    public void OnCollect(PlayerAmmunition Collector)
    {
        if (CurrentState != State.Dropped) return;
        CurrentState = State.Collected;
        owner = Collector;
        transform.SetLayerRecursive(LayerMask.NameToLayer("Player"));
        transform.SetParent(owner.transform, true);
        Shadow.SetActive(false);
        DroppedIndicator.SetActive(false);
        Rigidbody.isKinematic = false;
        StopCoroutine(nameof(DropAnimation));
        DropImpactVFX.gameObject.SetActive(false);
        Visual.transform.localPosition = default;
        Collider.isTrigger = false;
    }

    protected void Drop()
    {
        if (CurrentState != State.Shooting) return;
        Collider.isTrigger = true;
        transform.SetLayerRecursive(0);
        CurrentState = State.Dropped;
        launchStartTime = float.PositiveInfinity;
        Rigidbody.isKinematic = true;
        damageEvent = null;
        StartCoroutine(DropAnimation());
    }

    private void FixedUpdate()
    {
        if (CurrentState != State.Shooting) return;
        var t = FlightTime / 2f;
        t = 1 - (t * 2 - 1) * (t * 2 - 1);
        currentFlightHeightInPixels = Mathf.RoundToInt(Mathf.Lerp(minFlightHeightInPixels, maxflightHeightInPixels, t));
        Visual.transform.localPosition = Vector3.forward * currentFlightHeight;
        VelocityController.AddOverwriteMovement(new(ModifiedDirection._x0y().normalized * ModifiedSpeed + SpeedAtLaunch._x0y()), 0f, 0);
        if (FlightTime > 2) Drop();
    }

    private HealthEvent damageEvent;
    public void Shoot(Vector2 direction, Vector2 speedAtLaunch)
    {
        if (CurrentState != State.Collected) return;
        Visual.transform.localPosition = Vector3.forward * currentFlightHeight;
        transform.SetParent(null);
        transform.SetLayerRecursive(LayerMask.NameToLayer("PlayerProjectile"));
        Shadow.SetActive(true);
        owner = null;
        CurrentState = State.Shooting;
        Rigidbody.isKinematic = false;
        Rigidbody.linearVelocity = default;
        launchStartTime = Time.time;
        LaunchDirection = direction;
        SpeedAtLaunch = default;
        damageEvent = HealthEvent.Damage((uint)BaseDamage, source: gameObject);
    }
}
