using System;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth> {
  [SerializeField] private int maxHealth = 3;

  public int CurrentHealth { get; private set; }

  public event Action OnHealthChanged;

  protected override void Awake() {
    base.Awake();
    CurrentHealth = maxHealth;
  }

  public int MaxHealth => maxHealth;

  public void TakeDamage(int damage) {
    CurrentHealth -= damage;
    Debug.Log("Player took " + damage + " damage. Current health: " + CurrentHealth);

    if (CurrentHealth <= 0) {
      CurrentHealth = 0;
      Die();
    }

    OnHealthChanged.Invoke();
  }

  private void Die() {
    Debug.Log("Player died");
  }
}
