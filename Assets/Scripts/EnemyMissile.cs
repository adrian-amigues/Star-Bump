using UnityEngine;

public class EnemyMissile : MonoBehaviour {
  [SerializeField] public MissileData missileData;
  [SerializeField] private LayerMask bounceLayerMask;

  // private bool hasHitPlayer;
  private bool hasHitShield;
  private Rigidbody2D rb;
  private Transform target;
  private SpriteRenderer sr;
  private CircleCollider2D circleCollider;
  private Vector2 lastPosition;

  private void Awake() {
    rb = GetComponent<Rigidbody2D>();
    sr = GetComponent<SpriteRenderer>();
    circleCollider = GetComponent<CircleCollider2D>();
  }

  private void Start() {
    target = PlayerController.Instance.transform;
    sr.color = missileData.visualColor;

    LaunchMissile();
  }

  private void FixedUpdate() {
    if (hasHitShield) return;

    HomeTowardsTarget();
    lastPosition = transform.position;
  }

  void OnTriggerEnter2D(Collider2D other) {
    if (!hasHitShield && other.gameObject.CompareTag("Player")) {
      HandlePlayerCollision();
    } else if (other.gameObject.TryGetComponent(out PlayerShield shield)) {
      Debug.Log("Hit shield color: " + shield.shieldColor);

      if (shield.shieldColor == missileData.color) {
        HandleShieldBounce(shield.transform, shield.GetComponent<Collider2D>());
      }
    }
  }

  private void HandlePlayerCollision() {
    Debug.Log("Hit player");
    TriggerDestroyVfx();
    Destroy(gameObject);
  }

  private void LaunchMissile() {
    Vector2 direction = (target.position - transform.position).normalized;
    transform.up = direction;

    rb.AddForce(direction * missileData.speed, ForceMode2D.Impulse);
  }

  private void HomeTowardsTarget() {
    if (!target) return;

    Vector2 toTarget = (target.position - transform.position).normalized;

    // Apply a small steering force toward the player
    rb.AddForce(toTarget * missileData.acceleration, ForceMode2D.Force);

    if (rb.linearVelocity.magnitude > missileData.maxSpeed) {
      rb.linearVelocity = rb.linearVelocity.normalized * missileData.maxSpeed;
    }

    // TODO To use once there is a sprite?
    // Rotate missile to match velocity direction
    // if (rb.linearVelocity.sqrMagnitude > 0.01f) {
    //   float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
    //   rb.rotation = angle - 90f;
    // }
  }

  private void HandleShieldBounce(Transform shieldTransform, Collider2D shieldCollider) {
    hasHitShield = true;

    // Calculate the closest point on the shield to the missile
    // Vector2 closestPoint = shieldCollider.ClosestPoint(transform.position);
    // Vector2 missilePosition = transform.position;
    // Vector2 surfaceNormal = (missilePosition - closestPoint).normalized;

    // // Reflect the current velocity off the surface normal
    // Vector2 reflectedVelocity = Vector2.Reflect(rb.linearVelocity, surfaceNormal);


    var velocity = rb.linearVelocity;
    float speed = velocity.magnitude;
    if (speed <= 0.01f) return;

    // Use ray from last position (not current, since we might be inside)
    float castDistance = ((Vector2)transform.position - lastPosition).magnitude + circleCollider.radius;
    RaycastHit2D hit = Physics2D.CircleCast(transform.position, circleCollider.radius, velocity.normalized, castDistance, bounceLayerMask);

    if (hit.collider != null) {
      Vector2 reflected = Vector2.Reflect(velocity, hit.normal);

      Vector2 playerVelocity = PlayerController.Instance.Velocity;
      float playerImpactFactor = 1f; // may change later

      // rb.linearVelocity = reflected * playerImpactFactor;
      rb.linearVelocity = Vector2.zero;
      rb.AddForce(reflected + playerVelocity * playerImpactFactor, ForceMode2D.Impulse);

      rb.angularVelocity = Random.Range(-180f, 180f);

      // Slightly nudge missile out of the shield to prevent re-triggering
      transform.position = hit.point + hit.normal * circleCollider.radius * 1.1f;
    } else {
      // Fallback: just reflect using inverse of velocity (or destroy?)
      rb.linearVelocity = -velocity;
    }
    // rb.linearVelocity = reflectedVelocity + playerVelocity * playerImpactFactor;

    // rb.AddForce(reflectedVelocity + shieldVelocity * playerImpactFactor, ForceMode2D.Impulse);
  }

  private void TriggerDestroyVfx() {
    var particleSystem = missileData.destroyVfx.GetComponent<ParticleSystem>();
    var explosionEmissionArc = particleSystem.shape.arc;
    // This is so that the arc is centered on the vector opposite to the missile's direction
    var centerArcRotation = 180f + ((180f - explosionEmissionArc) / 2f);
    var vfxRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + centerArcRotation);

    var vfxInstance = Instantiate(missileData.destroyVfx, transform.position, vfxRotation);
    var particleMain = vfxInstance.GetComponent<ParticleSystem>().main;
    particleMain.startColor = new ParticleSystem.MinMaxGradient(sr.color);
  }
}
