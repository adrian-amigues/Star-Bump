using UnityEngine;

public class EnemyMissile : MonoBehaviour {
  [SerializeField] public MissileData missileData;

  // private bool hasHitPlayer;
  private bool hasHitShield;
  private Rigidbody2D rb;
  private Transform target;
  private SpriteRenderer sr;

  private void Awake() {
    rb = GetComponent<Rigidbody2D>();
    sr = GetComponent<SpriteRenderer>();
  }

  private void Start() {
    target = PlayerController.Instance.transform;
    sr.color = missileData.visualColor;
  }

  private void FixedUpdate() {
    if (hasHitShield) return;

    MoveTowardsTarget();
  }

  void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.CompareTag("Player")) {
      HandlePlayerCollision();
    } else if (other.gameObject.TryGetComponent(out PlayerShield shield)) {
      Debug.Log("Hit shield color: " + shield.shieldColor);

      if (shield.shieldColor == missileData.color) {
        HandleShieldBounce(shield.transform, shield.GetComponent<Collider2D>());
      }
    }
  }

  // private void OnCollisionEnter2D(Collision2D other) {
  //   if (other.gameObject.TryGetComponent(out PlayerShield shield)) {
  //     Debug.Log("Hit shield color: " + shield.shieldColor);
  //     if (shield.shieldColor == color) {
  //       hasHitShield = true;
  //     }
  //   }
  // }

  private void HandlePlayerCollision() {
    Debug.Log("Hit player");
    TriggerDestroyVfx();
    Destroy(gameObject);
  }

  private void HandleShieldBounce(Transform shieldTransform, Collider2D shieldCollider) {
    hasHitShield = true;

    // Calculate the closest point on the shield to the missile
    Vector2 closestPoint = shieldCollider.ClosestPoint(transform.position);
    Vector2 missilePosition = transform.position;

    // Calculate normal (surface direction) from the closest point
    Vector2 surfaceNormal = (missilePosition - closestPoint).normalized;

    // Reflect the current velocity off the surface normal
    Vector2 currentVelocity = rb.linearVelocity;
    Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, surfaceNormal);

    // Apply the reflected velocity with optional bounce damping
    float bounceDamping = 1f; // may change later
    rb.linearVelocity = reflectedVelocity * bounceDamping;

    // Optional: Add rotation
    rb.angularVelocity = Random.Range(-180f, 180f);
  }

  private void MoveTowardsTarget() {
    Vector2 direction = (target.position - transform.position).normalized;
    rb.linearVelocity = direction * missileData.speed;
    transform.up = direction;
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
