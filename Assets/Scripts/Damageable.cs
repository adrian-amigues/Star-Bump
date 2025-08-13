using System;
using TriInspector;
using UnityEngine;

public class Damageable : MonoBehaviour {
  [SerializeField] private bool showMiniHealthBar = false;
  [ShowIf("showMiniHealthBar")]
  [SerializeField] private Transform miniHealthBarPosition;

  [SerializeField] private bool shakeScreenOnDamage = false;
  [SerializeField] private int maxHealth = 3;

  public Action OnDeath;

  public int MaxHealth => maxHealth;
  public Transform MiniHealthBarPosition => miniHealthBarPosition;
  public int CurrentHealth { get; private set; }

  public event Action OnHealthChanged;

  private MiniHealthBarPresenter miniHealthBarPresenter;
  private bool isInvulnerable = false;

  private void Awake() {
    CurrentHealth = maxHealth;
  }

  private void OnEnable() {
    if (showMiniHealthBar) {
      miniHealthBarPresenter = FindFirstObjectByType<MiniHealthBarPresenter>();
      miniHealthBarPresenter?.RegisterDamageable(this);
    }
  }

  private void OnDisable() {
    if (showMiniHealthBar) {
      miniHealthBarPresenter?.UnregisterDamageable(this);
    }
  }

  private void HandleDeath() {
    OnDeath?.Invoke();
    miniHealthBarPresenter?.UnregisterDamageable(this);
  }

  public void TakeDamage(int damage) {
    if (isInvulnerable) return;

    CurrentHealth -= damage;

    if (CurrentHealth <= 0) {
      CurrentHealth = 0;
      HandleDeath();
    }

    if (shakeScreenOnDamage) {
      ScreenShakeManager.Instance.ShakeScreen();
    }
    OnHealthChanged?.Invoke();
  }

  public void SetIsInvulnerable(bool isInvulnerable) {
    this.isInvulnerable = isInvulnerable;
  }

  public void ToggleIsInvulnerable() {
    isInvulnerable = !isInvulnerable;
  }

  public void ResetHealth() {
    CurrentHealth = maxHealth;
    OnHealthChanged?.Invoke();
  }
}
