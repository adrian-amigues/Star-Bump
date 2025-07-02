using UnityEngine;

public class EnemyMissile : MonoBehaviour {
  [SerializeField] private MissileData missileData;

  public MissileData MissileData => missileData;

  private bool hasHitShield;
  private Rigidbody2D rb;
  private Transform target;
  private SpriteRenderer sr;
  private MissileTrajectoryDrawer trajectoryDrawer;

  private void Awake() {
    rb = GetComponent<Rigidbody2D>();
    sr = GetComponent<SpriteRenderer>();
    trajectoryDrawer = GetComponent<MissileTrajectoryDrawer>();
  }

  private void Start() {
    target = PlayerController.Instance.transform;

    LaunchMissile();
  }

  private void FixedUpdate() {
    if (!hasHitShield) {
      HomeTowardsTarget();
    }
    trajectoryDrawer.DrawTrajectory();
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
    } else if (other.gameObject.TryGetComponent(out EnemyMissile otherMissile)) {
      if (otherMissile.MissileData.color == MissileData.color) {
        HandleSameColorMissileCollision(otherMissile);
      }
    }
  }

  public void Initialize(MissileData data) {
    missileData = data;
    sr.color = missileData.visualColor;
  }

  private void HandlePlayerCollision() {
    PlayerHealth.Instance.TakeDamage(1);
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

    if (!hasHitShield && rb.linearVelocity.magnitude > MissileData.maxSpeed) {
      rb.linearVelocity = rb.linearVelocity.normalized * MissileData.maxSpeed;
    }
  }

  private void TriggerDestroyVfx() {
    var particleSystem = MissileData.destroyVfx.GetComponent<ParticleSystem>();
    var explosionEmissionArc = particleSystem.shape.arc;
    var direction = rb.linearVelocity.normalized;

    // The VFX arc can't be rotated by default, so we need to calculate the rotation here so that further computations are easier
    var arcRotationToCenterTop = (180f - explosionEmissionArc) / 2f;
    // Get the angle from the direction vector and add 180 degrees to face opposite
    var missileDirectionAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    var vfxRotation = Quaternion.Euler(0, 0, 180 - missileDirectionAngle + arcRotationToCenterTop);

    var vfxInstance = Instantiate(MissileData.destroyVfx, transform.position, vfxRotation);

    var particleMain = vfxInstance.GetComponent<ParticleSystem>().main;
    particleMain.startColor = new ParticleSystem.MinMaxGradient(sr.color);
  }

  private void HandleSameColorMissileCollision(EnemyMissile otherMissile) {
    TriggerDestroyVfx();
    Destroy(gameObject);
  }
}
