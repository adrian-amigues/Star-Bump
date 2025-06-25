using UnityEngine;

public class EnemyMissile : MonoBehaviour {
  [SerializeField] private MissileData missileData;

  public MissileData MissileData => missileData;

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

    LaunchMissile();
  }

  private void FixedUpdate() {
    if (hasHitShield) return;
    HomeTowardsTarget();
  }

  void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.CompareTag("Player")) {
      HandlePlayerCollision();
    }
  }

  void OnCollisionEnter2D(Collision2D other) {
    if (other.gameObject.TryGetComponent(out PlayerShield shield)) {
      Debug.Log("Hit shield color: " + shield.shieldColor);

      if (shield.shieldColor == MissileData.color) {
        hasHitShield = true;
      }
    }
  }

  public void Initialize(MissileData data) {
    missileData = data;
    sr.color = missileData.visualColor;
  }

  private void HandlePlayerCollision() {
    Debug.Log("Hit player");
    TriggerDestroyVfx();
    Destroy(gameObject);
  }

  private void LaunchMissile() {
    Vector2 direction = (target.position - transform.position).normalized;
    transform.up = direction;

    rb.AddForce(direction * MissileData.speed, ForceMode2D.Impulse);
  }

  private void HomeTowardsTarget() {
    if (!target) return;

    Vector2 toTarget = (target.position - transform.position).normalized;

    // Apply a small steering force toward the player
    rb.AddForce(toTarget * MissileData.acceleration, ForceMode2D.Force);

    if (rb.linearVelocity.magnitude > MissileData.maxSpeed) {
      rb.linearVelocity = rb.linearVelocity.normalized * MissileData.maxSpeed;
    }
  }

  private void TriggerDestroyVfx() {
    var particleSystem = MissileData.destroyVfx.GetComponent<ParticleSystem>();
    var explosionEmissionArc = particleSystem.shape.arc;
    // This is so that the arc is centered on the vector opposite to the missile's direction
    var centerArcRotation = 180f + ((180f - explosionEmissionArc) / 2f);
    var vfxRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + centerArcRotation);

    var vfxInstance = Instantiate(MissileData.destroyVfx, transform.position, vfxRotation);
    var particleMain = vfxInstance.GetComponent<ParticleSystem>().main;
    particleMain.startColor = new ParticleSystem.MinMaxGradient(sr.color);
  }
}
