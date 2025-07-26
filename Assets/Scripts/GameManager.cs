using System;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager> {
  public int CurrentLevel { get; private set; }

  public event Action OnLevelChanged;

  private MenuUIPresenter menuPresenter;
  private PlayerController player;
  private bool isRestarting = false;
  private bool isPaused = false;

  protected override void Awake() {
    base.Awake();
  }

  private void Update() {
    if (Keyboard.current.escapeKey.wasPressedThisFrame) {
      if (isPaused) {
        HandleContinueClicked();
      } else {
        OnPause();
      }
    }
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

  private void Init() {
    menuPresenter = FindFirstObjectByType<MenuUIPresenter>();
    player = FindFirstObjectByType<PlayerController>();

    player.GetComponentInChildren<PlayerDeathEffects>().OnPlayerExploded += HandlePlayerExploded;
    menuPresenter.OnTryAgainClicked += HandleTryAgainClicked;
    menuPresenter.OnStartClicked += HandleStartClicked;
    menuPresenter.OnExitClicked += HandleExitClicked;
    menuPresenter.OnContinueClicked += HandleContinueClicked;

    CurrentLevel = 1;

    if (isRestarting) {
      isRestarting = false;
      HandleStartClicked();
    } else {
      menuPresenter.ShowMainMenu();
      Time.timeScale = 0;
      CursorManager.Instance.SetCursorLockState(false);
    }
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
    OnLevelChanged?.Invoke();
  }

  private void HandleExitClicked() {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  private void HandleTryAgainClicked() {
    isRestarting = true;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  private void OnPause() {
    isPaused = true;
    menuPresenter.ShowPauseMenu();
    Time.timeScale = 0;
    CursorManager.Instance.SetCursorLockState(false);
  }

  private void HandleContinueClicked() {
    Debug.Log("Continue clicked");
    isPaused = false;
    menuPresenter.Clear();
    CursorManager.Instance.SetCursorLockState(true);
    Time.timeScale = 1;
    OnLevelChanged?.Invoke();
  }
}
