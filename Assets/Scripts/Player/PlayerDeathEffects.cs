using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class PlayerDeathEffects : MonoBehaviour {
  [SerializeField] private GameObject[] deathVfxList;
  [SerializeField] private GameObject[] rendererList;
  [SerializeField] private float fadeOutDuration = 1f;

  private PlayerShield[] playerShields;

  public Action OnPlayerExploded;

  private void Start() {
    GetComponent<Damageable>().OnDeath += HandlePlayerDeath;
    playerShields = rendererList.Select(r => r.GetComponent<PlayerShield>())
      .Where(s => s != null)
      .ToArray();
  }


  private void HandlePlayerDeath() {
    StartCoroutine(MainPlayerDeathRoutine());
  }

  private IEnumerator MainPlayerDeathRoutine() {
    StartCoroutine(FadeOutRoutine());
    yield return new WaitForSeconds(1f);

    foreach (var vfx in deathVfxList) {
      var particleSystem = vfx.GetComponent<ParticleSystem>();
      particleSystem?.Play();
    }
    OnPlayerExploded?.Invoke();
    ScreenShakeManager.Instance.ShakeScreen();
  }

  private IEnumerator FadeOutRoutine() {
    float elapsed = 0f;
    var spriteRenderers = rendererList.Select(r => r.GetComponent<SpriteRenderer>()).ToArray();

    foreach (var shield in playerShields) {
      shield.DisableAnimator();
    }

    while (elapsed < fadeOutDuration) {
      elapsed += Time.deltaTime;
      float progress = elapsed / fadeOutDuration;
      float alpha = Mathf.Lerp(1, 0, progress);

      foreach (var spriteRenderer in spriteRenderers) {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
      }
      yield return null;
    }
  }
}
