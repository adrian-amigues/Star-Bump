using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour {
  [SerializeField] public MissileColor shieldColor = MissileColor.Pink;
  [SerializeField] public float shieldCooldown = 3f;

  private Animator animator;
  private Collider2D shieldCollider;

  private void Awake() {
    animator = GetComponent<Animator>();
    shieldCollider = GetComponent<Collider2D>();
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.TryGetComponent(out EnemyMissile missile)) {
      Debug.Log("Shield hit missile: " + missile.MissileData.color);
      if (missile.MissileData.color != shieldColor) {
        Debug.Log("Shield hit shield: " + shieldColor);
        animator.SetTrigger("Deactivate");
      }
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
}
