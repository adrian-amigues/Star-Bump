using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
  [SerializeField] public MissileColor shieldColor = MissileColor.Pink;
  [SerializeField] public float shieldCooldown = 3f;

  private Animator animator;
  private Collider2D shieldCollider;
  private PlayerController player;

  private void Awake() {
    animator = GetComponent<Animator>();
    shieldCollider = GetComponent<Collider2D>();
    player = GetComponentInParent<PlayerController>();
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.TryGetComponent(out EnemyMissile missile)) {
      if (missile.MissileData.color != shieldColor) {
        animator.SetTrigger("Deactivate");
      }
    }
  }

  private void OnCollisionExit2D(Collision2D collision) {
    if (collision.gameObject.TryGetComponent(out EnemyMissile missile)) {
      if (missile.MissileData.color == shieldColor) {
        AddMomentumToMissileAfterBounce(missile);
      }
    }
  }

  private void AddMomentumToMissileAfterBounce(EnemyMissile missile) {
    var missileRb = missile.GetComponent<Rigidbody2D>();
    Vector2 playerVelocity = player.CurrentVelocity;
    Vector2 missileDirection = missileRb.linearVelocity.normalized;

    // Check if player velocity aligns with missile direction
    float alignment = Vector2.Dot(playerVelocity, missileDirection);

    if (alignment > 0) {
      float momentumTransfer = 0.7f * alignment;
      missileRb.linearVelocity += missileDirection * momentumTransfer;
    }
  }

  private IEnumerator ShieldCooldownRoutine() {
    yield return new WaitForSeconds(shieldCooldown);
    animator.SetTrigger("Activate");
  }

  public void OnDeactivateAnimationEnd() {
    shieldCollider.enabled = false;
    StartCoroutine(ShieldCooldownRoutine());
  }

  public void OnActivateAnimationEnd() {
    shieldCollider.enabled = true;
  }

  public void DisableAnimator() {
    animator.enabled = false;
  }

  public void EnableAnimator() {
    animator.enabled = true;
  }
}
