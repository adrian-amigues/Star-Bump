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

  public void Clear() {
    root.Clear();
  }


  public void ShowMainMenu() {
    var container = ShowMenu(mainMenuTemplate);
    var startButton = container.Q<Button>("startButton");
    var exitButton = container.Q<Button>("exitButton");

    startButton.clicked += () => ButtonClick(OnStartClicked);
    exitButton.clicked += () => ButtonClick(OnExitClicked);

    CursorManager.Instance.SetCursorLockState(false);
  }

  public void ShowGameOver() {
    var container = ShowMenu(gameOverTemplate);
    var tryAgainButton = container.Q<Button>("tryAgainButton");
    var exitButton = container.Q<Button>("exitButton");

    tryAgainButton.clicked += () => ButtonClick(OnTryAgainClicked);
    exitButton.clicked += () => ButtonClick(OnExitClicked);

    CursorManager.Instance.SetCursorLockState(false);
  }

  public void ShowPauseMenu() {
    var container = ShowMenu(pauseMenuTemplate);
    var continueButton = container.Q<Button>("continueButton");
    var exitButton = container.Q<Button>("exitButton");

    continueButton.clicked += () => ButtonClick(OnContinueClicked);
    exitButton.clicked += () => ButtonClick(OnExitClicked);
  }

  public void ShowLevelCompleted(int levelIndex) {
    var container = ShowMenu(levelCompletedTemplate);
    var continueButton = container.Q<Button>("continueButton");
    var exitButton = container.Q<Button>("exitButton");

    var levelLabel = container.Q<Label>("levelLabel");
    levelLabel.text = $"Level {levelIndex}";

    continueButton.clicked += () => ButtonClick(OnNextLevelClicked);
    exitButton.clicked += () => ButtonClick(OnExitClicked);
  }

  public void ShowGameWon() {
    var container = ShowMenu(gameWonTemplate);
    var mainMenuButton = container.Q<Button>("mainMenuButton");
    var exitButton = container.Q<Button>("exitButton");

    mainMenuButton.clicked += () => ButtonClick(OnMainMenuClicked);
    exitButton.clicked += () => ButtonClick(OnExitClicked);
  }
}