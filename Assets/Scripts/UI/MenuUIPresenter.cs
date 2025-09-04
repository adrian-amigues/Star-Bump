using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUIPresenter : MonoBehaviour {
  [SerializeField] private VisualTreeAsset mainMenuTemplate;
  [SerializeField] private VisualTreeAsset gameOverTemplate;
  [SerializeField] private VisualTreeAsset pauseMenuTemplate;
  [SerializeField] private VisualTreeAsset levelCompletedTemplate;
  [SerializeField] private VisualTreeAsset gameWonTemplate;

  private VisualElement root;

  public event Action OnStartClicked;
  public event Action OnExitClicked;
  public event Action OnTryAgainClicked;
  public event Action OnContinueClicked;
  public event Action OnNextLevelClicked;
  public event Action OnMainMenuClicked;

  private void Awake() {
    root = GetComponent<UIDocument>().rootVisualElement;
  }

  private TemplateContainer ShowMenu(VisualTreeAsset template) {
    root.Clear();
    var container = template.CloneTree();
    container.style.height = new StyleLength(Length.Percent(100));
    root.Add(container);
    return container;
  }

  private void ButtonClick(Action action) {
    action?.Invoke();
    SoundManager.PlaySound(SoundType.ButtonClick);
  }

  private void ConditionalExitButton(Button button) {
#if UNITY_WEBGL
    button.style.display = DisplayStyle.None;
#else
    button.clicked += () => ButtonClick(OnExitClicked);
#endif
  }

  public void Clear() {
    root.Clear();
  }


  public void ShowMainMenu() {
    var container = ShowMenu(mainMenuTemplate);
    var startButton = container.Q<Button>("startButton");
    var exitButton = container.Q<Button>("exitButton");

    startButton.clicked += () => ButtonClick(OnStartClicked);
    ConditionalExitButton(exitButton);

    CursorManager.Instance.SetCursorLockState(false);
  }

  public void ShowGameOver() {
    var container = ShowMenu(gameOverTemplate);
    var tryAgainButton = container.Q<Button>("tryAgainButton");
    var mainMenuButton = container.Q<Button>("mainMenuButton");

    tryAgainButton.clicked += () => ButtonClick(OnTryAgainClicked);
    mainMenuButton.clicked += () => ButtonClick(OnMainMenuClicked);

    CursorManager.Instance.SetCursorLockState(false);
  }

  public void ShowPauseMenu() {
    var container = ShowMenu(pauseMenuTemplate);
    var continueButton = container.Q<Button>("continueButton");
    var mainMenuButton = container.Q<Button>("mainMenuButton");

    continueButton.clicked += () => ButtonClick(OnContinueClicked);
    mainMenuButton.clicked += () => ButtonClick(OnMainMenuClicked);
  }

  public void ShowLevelCompleted(int levelIndex) {
    var container = ShowMenu(levelCompletedTemplate);
    var continueButton = container.Q<Button>("continueButton");
    var mainMenuButton = container.Q<Button>("mainMenuButton");

    var levelLabel = container.Q<Label>("levelLabel");
    levelLabel.text = $"Level {levelIndex}";

    continueButton.clicked += () => ButtonClick(OnNextLevelClicked);
    mainMenuButton.clicked += () => ButtonClick(OnMainMenuClicked);
  }

  public void ShowGameWon() {
    var container = ShowMenu(gameWonTemplate);
    var mainMenuButton = container.Q<Button>("mainMenuButton");
    var exitButton = container.Q<Button>("exitButton");

    mainMenuButton.clicked += () => ButtonClick(OnMainMenuClicked);
    ConditionalExitButton(exitButton);
  }
}