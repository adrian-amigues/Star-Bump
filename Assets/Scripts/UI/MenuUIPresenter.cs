using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUIPresenter : MonoBehaviour {
  [SerializeField] private VisualTreeAsset mainMenuTemplate;
  [SerializeField] private VisualTreeAsset gameOverTemplate;

  private VisualElement root;

  public event Action OnTryAgainClicked;

  private void Awake() {
    root = GetComponent<UIDocument>().rootVisualElement;
  }

  public void ShowMainMenu() {
    root.Clear();
    var mainMenu = mainMenuTemplate.CloneTree();
    root.Add(mainMenu);
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