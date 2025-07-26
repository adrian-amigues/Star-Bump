using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUIPresenter : MonoBehaviour {
  [SerializeField] private VisualTreeAsset mainMenuTemplate;
  [SerializeField] private VisualTreeAsset gameOverTemplate;
  [SerializeField] private VisualTreeAsset pauseMenuTemplate;

  private VisualElement root;

  public event Action OnStartClicked;
  public event Action OnExitClicked;
  public event Action OnTryAgainClicked;
  public event Action OnContinueClicked;

  private void Awake() {
    root = GetComponent<UIDocument>().rootVisualElement;
  }

  public void Clear() {
    root.Clear();
  }

  public void ShowMainMenu() {
    root.Clear();
    var container = mainMenuTemplate.CloneTree();
    container.style.height = new StyleLength(Length.Percent(100));
    root.Add(container);

    var startButton = container.Q<Button>("startButton");
    var exitButton = container.Q<Button>("exitButton");

    startButton.clicked += () => OnStartClicked?.Invoke();
    exitButton.clicked += () => OnExitClicked?.Invoke();

    CursorManager.Instance.SetCursorLockState(false);
  }

  public void ShowGameOver() {
    root.Clear();
    var container = gameOverTemplate.CloneTree();
    container.style.height = new StyleLength(Length.Percent(100));
    root.Add(container);

    var tryAgainButton = container.Q<Button>("tryAgainButton");
    var exitButton = container.Q<Button>("exitButton");

    tryAgainButton.clicked += () => OnTryAgainClicked?.Invoke();
    exitButton.clicked += () => OnExitClicked?.Invoke();

    CursorManager.Instance.SetCursorLockState(false);
  }

  public void ShowPauseMenu() {
    root.Clear();
    var container = pauseMenuTemplate.CloneTree();
    container.style.height = new StyleLength(Length.Percent(100));
    root.Add(container);

    var continueButton = container.Q<Button>("continueButton");
    var exitButton = container.Q<Button>("exitButton");

    continueButton.clicked += () => OnContinueClicked?.Invoke();
    exitButton.clicked += () => OnExitClicked?.Invoke();
  }
}