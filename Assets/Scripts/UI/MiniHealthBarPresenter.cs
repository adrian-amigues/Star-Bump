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

  private void Update() {
    // Update positions for all active health bars
    foreach (var kvp in activeHealthBars) {
      Damageable damageable = kvp.Key;
      VisualElement healthBarElement = kvp.Value;
      Debug.Log($"Health bar element: {healthBarElement}");

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

    if (miniHealthBarContainer == null) {
      Debug.LogError("MiniHealthBarContainer still null after InitializeUI()!");
      Debug.LogError($"UIDocument: {uiDocument}");
      Debug.LogError($"RootElement: {rootElement}");
      return;
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

    Vector3 healthBarWorldPos;
    if (damageable.MiniHealthBarPosition != null) {
      healthBarWorldPos = damageable.MiniHealthBarPosition.position;
    } else {
      float healthBarWidth = healthBarElement.resolvedStyle.width;
      float healthBarHeight = healthBarElement.resolvedStyle.height;

      Vector3 centeredPosition = damageable.transform.position + Vector3.up * defaultMiniHealthBarOffset;

      healthBarWorldPos = new Vector3(
        centeredPosition.x - (healthBarWidth / 2f),
        centeredPosition.y - (healthBarHeight / 2f),
        centeredPosition.z
      );
    }

    Vector3 screenPos = mainCamera.WorldToScreenPoint(healthBarWorldPos);

    healthBarElement.style.left = screenPos.x;
    // UI Toolkit uses top-left origin, Unity screen uses bottom-left
    healthBarElement.style.top = Screen.height - screenPos.y;
  }

  public void ClearAllHealthBars() {
    var damageables = activeHealthBars.Keys.ToList();
    foreach (var damageable in damageables) {
      UnregisterDamageable(damageable);
    }
  }
}
