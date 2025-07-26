using System;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
  public int CurrentLevel { get; private set; }

  public event Action OnLevelChanged;

  private MenuUIPresenter menuPresenter;
  private PlayerController player;

  protected override void Awake() {
    base.Awake();
  }

  private void OnEnable() {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnDisable() {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    Init();
  }

  private async void Init() {
    menuPresenter = FindFirstObjectByType<MenuUIPresenter>();
    player = FindFirstObjectByType<PlayerController>();

    player.GetComponentInChildren<PlayerDeathEffects>().OnPlayerExploded += HandlePlayerExploded;
    menuPresenter.OnTryAgainClicked += HandleTryAgainClicked;
    menuPresenter.OnStartClicked += HandleStartClicked;
    menuPresenter.OnExitClicked += HandleExitClicked;

    CurrentLevel = 1;

    menuPresenter.ShowMainMenu();
    Time.timeScale = 0;
  }

  public void NextLevel() {
    CurrentLevel++;
    OnLevelChanged?.Invoke();
  }

  private async void HandlePlayerExploded() {
    await UniTask.Delay(1000);
    menuPresenter.ShowGameOver();
  }

  private void HandleStartClicked() {
    menuPresenter.Clear();
    CursorManager.Instance.SetCursorLockState(true);
    Time.timeScale = 1;
  }

  private void HandleExitClicked() {
    Application.Quit();
  }

  private void HandleTryAgainClicked() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    Destroy(player);
  }
}
