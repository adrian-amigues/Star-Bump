using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIPresenter : MonoBehaviour {
  [SerializeField] private UIDocument uiDocument;

  private ProgressBar healthBar;

  private Damageable playerHealth;

  private void Start() {
    InitializeUI();
    InitializeModels();
  }

  private void UpdateHealthUI() {
    if (playerHealth == null || healthBar == null) return;

    float percentage = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth * 100;
    healthBar.value = percentage;
  }

  private void InitializeUI() {
    var root = uiDocument.rootVisualElement;
    healthBar = root.Q<ProgressBar>("HealthBar");
  }

  public void InitializeModels() {
    playerHealth = FindFirstObjectByType<PlayerController>().GetComponentInChildren<Damageable>();
    playerHealth.OnHealthChanged += UpdateHealthUI;

    UpdateHealthUI();
  }
}
