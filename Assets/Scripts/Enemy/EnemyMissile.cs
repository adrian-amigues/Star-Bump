using UnityEngine;

public class EnemyMissile : MonoBehaviour {
  [SerializeField] private MissileData missileData;
  [SerializeField] private GameObject gameBoundsParticleSpawner;

  public MissileData MissileData => missileData;

  private bool hasHitShield;
  private Vector2 lastVelocity;
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
    if (GameManager.Instance.TryGetPlayer(out var player)) {
      target = player;
    }
    LaunchMissile();
  }

  private void FixedUpdate() {
    if (!hasHitShield) {
      // HomeTowardsTarget();
      LimitSpeed();
    }
    lastVelocity = rb.linearVelocity;
    trajectoryDrawer?.DrawTrajectory();
  }

  void OnTriggerExit2D(Collider2D collision) {
    if (!enabled || !collision.gameObject.activeInHierarchy || !this.gameObject.activeInHierarchy) return;

    if (collision.gameObject.CompareTag("GameAreaBoundary")) {
      var gameAreaCenter = collision.transform.position;
      var directionToCenter = gameAreaCenter - transform.position;
      var angle = -Mathf.Atan2(directionToCenter.x, directionToCenter.y) * Mathf.Rad2Deg;

      Instantiate(gameBoundsParticleSpawner, transform.position, Quaternion.Euler(0, 0, angle));
      HandleMissileCollision();
      SoundManager.PlaySound(SoundType.MissileHitBarrier);
    }
  }

  void OnCollisionEnter2D(Collision2D other) {
    if (other.collider.TryGetComponent(out PlayerShield shield)) {
      if (shield.shieldColor == MissileData.color) {
        hasHitShield = true;
        SoundManager.PlaySound(SoundType.MissileBounce);
      } else {
        SoundManager.PlaySound(SoundType.MissileBounceWrongShield);
      }
    } else if (other.collider.TryGetComponent(out EnemyMissile otherMissile)) {
      // if (otherMissile.MissileData.color == MissileData.color) {
      HandleMissileCollision();
      SoundManager.PlaySound(SoundType.MissileExplosion);
      // }
    } else if (other.collider.TryGetComponent(out Damageable damageable)) {
      damageable.TakeDamage(missileData.damage);
      HandleMissileCollision();
      if (other.collider.tag == "Player") {
        SoundManager.PlaySound(SoundType.MissileHit);
      } else {
        SoundManager.PlaySound(SoundType.EnemyHit);
      }
    }
  }

  private void LaunchMissile() {
    if (target == null) return;
    Vector2 direction = (target.position - transform.position).normalized;
    transform.up = direction;

    rb.AddForce(direction * MissileData.speed, ForceMode2D.Impulse);
  }

  private void HomeTowardsTarget() {
    if (!target) return;
    Vector2 toTarget = (target.position - transform.position).normalized;
    rb.AddForce(toTarget * MissileData.acceleration, ForceMode2D.Force);
  }

  private void LimitSpeed() {
    if (rb.linearVelocity.magnitude > MissileData.maxSpeed) {
      rb.linearVelocity = rb.linearVelocity.normalized * MissileData.maxSpeed;
    }
  }

  private void TriggerDestroyVfx() {
    var direction = lastVelocity.normalized;

    // Get the angle from the direction vector and add 180 degrees to face opposite
    var missileDirectionAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    var vfxRotation = Quaternion.Euler(0, 0, 180 - missileDirectionAngle);

    var vfxInstance = Instantiate(MissileData.destroyVfx, transform.position, vfxRotation);

    var particleMain = vfxInstance.GetComponent<ParticleSystem>().main;
    particleMain.startColor = new ParticleSystem.MinMaxGradient(sr.color);
  }

  private void HandleMissileCollision() {
    TriggerDestroyVfx();
    Destroy(gameObject);
  }
}
