using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniHealthBarPresenter : MonoBehaviour {
  [SerializeField] private UIDocument uiDocument;
  [SerializeField] private VisualTreeAsset miniHealthBarTemplate;
  [SerializeField] private float defaultMiniHealthBarOffset = 0.5f;

  private VisualElement rootElement;
  private VisualElement miniHealthBarContainer;
  private Camera mainCamera;

  private Dictionary<Damageable, VisualElement> activeHealthBars = new Dictionary<Damageable, VisualElement>();

  private void Awake() {
    InitializeUI();
    mainCamera = Camera.main;
  }

  private void LateUpdate() {
    // Update positions for all active health bars
    foreach (var kvp in activeHealthBars) {
      Damageable damageable = kvp.Key;
      VisualElement healthBarElement = kvp.Value;

      UpdateHealthBarPosition(damageable, healthBarElement);
    }
  }

  private void InitializeUI() {
    rootElement = uiDocument.rootVisualElement;
    miniHealthBarContainer = rootElement.Q<VisualElement>("MiniHealthBarContainer");
  }

  public void RegisterDamageable(Damageable damageable) {
    if (activeHealthBars.ContainsKey(damageable)) return;

    if (miniHealthBarContainer == null) {
      InitializeUI();
    }

    var healthBarClone = miniHealthBarTemplate.Instantiate();
    healthBarClone.style.display = DisplayStyle.Flex; // Make it visible

    miniHealthBarContainer.Add(healthBarClone);
    activeHealthBars[damageable] = healthBarClone;

    damageable.OnHealthChanged += () => UpdateHealthBarView(damageable);
    UpdateHealthBarView(damageable);
  }

  public void UnregisterDamageable(Damageable damageable) {
    if (!activeHealthBars.TryGetValue(damageable, out var healthBarElement)) return;

    damageable.OnHealthChanged -= () => UpdateHealthBarView(damageable);

    miniHealthBarContainer.Remove(healthBarElement);
    activeHealthBars.Remove(damageable);
  }

  private void UpdateHealthBarView(Damageable damageable) {
    if (!activeHealthBars.TryGetValue(damageable, out var healthBarElement)) return;

    // Find the progress bar within the cloned element
    var progressBar = healthBarElement.Q<ProgressBar>();
    if (progressBar == null) return;

    float healthPercentage = (float)damageable.CurrentHealth / damageable.MaxHealth * 100f;
    progressBar.value = healthPercentage;
  }

  private void UpdateHealthBarPosition(Damageable damageable, VisualElement healthBarElement) {
    if (damageable == null || healthBarElement == null || mainCamera == null) return;

    var progressBar = healthBarElement.Q<ProgressBar>();
    if (progressBar == null) return;

    Vector3 worldPosition;
    if (damageable.MiniHealthBarPosition != null) {
      worldPosition = damageable.MiniHealthBarPosition.position;
    } else {
      worldPosition = damageable.transform.position + Vector3.up * defaultMiniHealthBarOffset;
    }

    // Convert world position directly to panel coordinates
    Vector2 panelPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
      rootElement.panel,
      worldPosition,
      mainCamera
    );

    // Get health bar dimensions in pixels
    float healthBarWidth = progressBar.resolvedStyle.width;
    float healthBarHeight = progressBar.resolvedStyle.height;

    // Center the health bar by offsetting position by half its dimensions
    float centeredX = panelPosition.x - (healthBarWidth / 2f);
    float centeredY = panelPosition.y - (healthBarHeight / 2f);
    healthBarElement.style.left = centeredX;
    healthBarElement.style.top = centeredY;
  }

  public void ClearAllHealthBars() {
    var damageables = activeHealthBars.Keys.ToList();
    foreach (var damageable in damageables) {
      UnregisterDamageable(damageable);
    }
  }
}
