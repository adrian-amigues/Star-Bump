using UnityEngine;
using UnityEngine.UIElements;

public class SplashTextUIPresenter : MonoBehaviour {
  [SerializeField] private UIDocument uiDocument;

  private Label splashText;

  private void Start() {
    InitializeUI();
    InitializeModels();
  }

  private void InitializeUI() {
    var root = uiDocument.rootVisualElement;
    splashText = root.Q<Label>();
  }

  private void InitializeModels() {
    GameManager.Instance.OnLevelChanged += UpdateSplashText;
    UpdateSplashText();
  }

  private void UpdateSplashText() {
    var currentLevel = GameManager.Instance.CurrentLevel;
    splashText.text = $"Level {currentLevel}";
  }
}
