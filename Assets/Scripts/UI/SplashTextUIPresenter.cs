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
    LevelManager.Instance.OnLevelChanged += UpdateSplashText;
  }

  private void UpdateSplashText() {
    var currentLevel = LevelManager.Instance.CurrentLevel;
    splashText.text = $"Level {currentLevel}";
    ShowSplashText();
  }

  private void HideSplashText() {
    splashText.style.display = DisplayStyle.None;
  }

  private void ShowSplashText() {
    splashText.style.display = DisplayStyle.Flex;
  }
}
