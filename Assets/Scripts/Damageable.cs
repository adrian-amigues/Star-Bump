using System;
using TriInspector;
using UnityEngine;

public class Damageable : MonoBehaviour {
  [SerializeField] private bool showMiniHealthBar = false;
  [ShowIf("showMiniHealthBar")]
  [SerializeField] private Transform miniHealthBarPosition;

  [SerializeField] private bool shakeScreenOnDamage = false;
  [SerializeField] private int maxHealth = 3;

  public int MaxHealth => maxHealth;
  public Transform MiniHealthBarPosition => miniHealthBarPosition;
  public int CurrentHealth { get; private set; }

  public event Action OnHealthChanged;

  private MiniHealthBarPresenter miniHealthBarPresenter;

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

  public void TakeDamage(int damage) {
    CurrentHealth -= damage;
    Debug.Log("Damageable took " + damage + " damage. Current health: " + CurrentHealth);

    if (CurrentHealth <= 0) {
      CurrentHealth = 0;
      HandleDeath();
    }

    if (shakeScreenOnDamage) {
      ScreenShakeManager.Instance.ShakeScreen();
    }
    OnHealthChanged?.Invoke();
  }

  private void HandleDeath() {
    Debug.Log("Damageable died");
  }
}
