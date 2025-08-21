using System.Collections;
using System.Linq;
using System;
using UnityEngine;
using PrimeTween;
using Shapes;
using MoreMountains.Feedbacks;

public class PlayerDeathEffects : MonoBehaviour {
  [SerializeField] private GameObject[] deathVfxList;
  [SerializeField] private GameObject[] rendererList;
  [SerializeField] private float fadeOutDuration = 1f;
  [SerializeField] private MMF_Player deathFeedback;

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
  }

  private void ExecuteDeathSequence() {
    DisableAllColliders();
    DisableShieldAnimators();

    var fadeGroup = Sequence.Create();
    foreach (var r in rendererList) {
      if (r.TryGetComponent<ShapeRenderer>(out var shapeRenderer)) {
        fadeGroup.Group(Tween.Custom(shapeRenderer.Color.a, 0, fadeOutDuration, (a) => {
          var color = shapeRenderer.Color;
          color.a = a;
          shapeRenderer.Color = color;
        }));
      } else if (r.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) {
        fadeGroup.Group(Tween.Alpha(spriteRenderer, 0, fadeOutDuration));
      }
    }

    Sequence.Create()
      .Chain(fadeGroup)
      .OnComplete(() => {
        deathFeedback?.PlayFeedbacks();
        TriggerExplosionEffects();
        OnPlayerExploded?.Invoke();
        ScreenShakeManager.Instance.ShakeScreen();
        Destroy(gameObject);
      });
  }

  private void TriggerExplosionEffects() {
    SoundManager.PlaySound(SoundType.PlayerExplosion);
    foreach (var vfx in deathVfxList) {
      var particleSystem = vfx.GetComponent<ParticleSystem>();
      vfx.transform.SetParent(null);
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
