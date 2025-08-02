using System.Collections;
using System.Linq;
using System;
using UnityEngine;
using PrimeTween;

public class PlayerDeathEffects : MonoBehaviour {
  [SerializeField] private GameObject[] deathVfxList;
  [SerializeField] private GameObject[] rendererList;
  [SerializeField] private float fadeOutDuration = 1f;

  private PlayerShield[] playerShields;

  public Action OnPlayerExploded;

  private void Start() {
    GetComponentInChildren<Damageable>().OnDeath += HandlePlayerDeath;
    playerShields = rendererList.Select(r => r.GetComponent<PlayerShield>())
      .Where(s => s != null)
      .ToArray();
  }


  private void HandlePlayerDeath() {
    ExecuteDeathSequence();
    // StartCoroutine(MainPlayerDeathRoutine());
  }

  private void ExecuteDeathSequence() {
    DisableAllColliders();
    DisableShieldAnimators();

    var fadeGroup = Sequence.Create();
    foreach (var r in rendererList) {
      var spriteRenderer = r.GetComponent<SpriteRenderer>();
      fadeGroup.Group(Tween.Alpha(spriteRenderer, 0, fadeOutDuration));
    }

    Sequence.Create()
      .Chain(fadeGroup)
      .OnComplete(() => {
        TriggerExplosionEffects();
        OnPlayerExploded?.Invoke();
        ScreenShakeManager.Instance.ShakeScreen();
        Destroy(gameObject);
      });
  }

  // private IEnumerator MainPlayerDeathRoutine() {
  //   DisableAllColliders();
  //   StartCoroutine(FadeOutRoutine());
  //   yield return new WaitForSeconds(fadeOutDuration);

  //   TriggerExplosionEffects();
  //   OnPlayerExploded?.Invoke();
  //   ScreenShakeManager.Instance.ShakeScreen();
  // }

  // private IEnumerator FadeOutRoutine() {
  //   float elapsed = 0f;
  //   var spriteRenderers = rendererList.Select(r => r.GetComponent<SpriteRenderer>()).ToArray();

  //   DisableShieldAnimators();

  //   while (elapsed < fadeOutDuration) {
  //     elapsed += Time.deltaTime;
  //     float progress = elapsed / fadeOutDuration;
  //     float alpha = Mathf.Lerp(1, 0, progress);

  //     foreach (var spriteRenderer in spriteRenderers) {
  //       var color = spriteRenderer.color;
  //       color.a = alpha;
  //       spriteRenderer.color = color;
  //     }
  //     yield return null;
  //   }
  // }

  private void TriggerExplosionEffects() {
    foreach (var vfx in deathVfxList) {
      var particleSystem = vfx.GetComponent<ParticleSystem>();
      particleSystem?.Play();
    }
  }

  private void DisableAllColliders() {
    var colliders = GetComponentsInChildren<Collider2D>();
    foreach (var collider in colliders) {
      collider.enabled = false;
    }
  }

  private void EnableAllColliders() {
    var colliders = GetComponentsInChildren<Collider2D>();
    foreach (var collider in colliders) {
      collider.enabled = true;
    }
  }

  private void DisableShieldAnimators() {
    foreach (var shield in playerShields) {
      shield.DisableAnimator();
    }
  }

  private void EnableShieldAnimators() {
    foreach (var shield in playerShields) {
      shield.EnableAnimator();
    }
  }

  public void ResetPlayerVisuals() {
    EnableAllColliders();
    EnableShieldAnimators();
    foreach (var r in rendererList) {
      var spriteRenderer = r.GetComponent<SpriteRenderer>();
      // Tween.Alpha(spriteRenderer, 1, 0);
      spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
    }
  }
}
