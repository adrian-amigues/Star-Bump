using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUIPresenter : MonoBehaviour {
  [SerializeField] private VisualTreeAsset mainMenuTemplate;
  [SerializeField] private VisualTreeAsset gameOverTemplate;

  private VisualElement root;

  public event Action OnStartClicked;
  public event Action OnExitClicked;
  public event Action OnTryAgainClicked;

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

    var gameOverButton = container.Q<Button>();
    gameOverButton.clicked += () => OnTryAgainClicked?.Invoke();

    CursorManager.Instance.SetCursorLockState(false);
  }

}