using System;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager> {
  public int CurrentLevel { get; private set; }
  public bool IsPlayerDead { get; private set; }
  public GameState State { get; private set; }

  public event Action OnLevelChanged;
  public enum GameState {
    None,
    MainMenu,
    Playing,
    Paused,
    GameOver,
    LevelCompleted,
    GameWon
  }

  private MenuUIPresenter menuPresenter;
  private PlayerController player;
  private PlayerDeathEffects playerDeathEffects;
  private TimeManager timeManager;

  // private GameState currentState;

  protected override void Awake() {
    base.Awake();
    Debug.Log("Awake " + State);
    // State = GameState.MainMenu;
    timeManager = GetComponent<TimeManager>();
  }

  private void Update() {
    if (Keyboard.current.escapeKey.wasPressedThisFrame) {
      switch (State) {
        case GameState.Playing:
          ChangeState(GameState.Paused);
          break;
        case GameState.Paused:
          ChangeState(GameState.Playing);
          break;
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
    InitUICallbacks();
    InitLevels();

    player = FindFirstObjectByType<PlayerController>();
    playerDeathEffects = player.GetComponentInChildren<PlayerDeathEffects>();
    playerDeathEffects.OnPlayerExploded += HandlePlayerExploded;
    IsPlayerDead = false;

    switch (State) {
      case GameState.GameOver:
        ChangeState(GameState.Playing);
        break;
      case GameState.MainMenu:
      case GameState.None:
      default:
        ChangeState(GameState.MainMenu);
        break;
    }
  }

  private void ChangeState(GameState newState) {
    if (newState == State) return;

    var previousState = State;
    ExitState(previousState);
    State = newState;
    EnterState(newState);
    // OnStateChanged?.Invoke(previousState, newState);
  }

  private void ExitState(GameState state) {
    // TODO: needed?
    // switch (state) {
    //   case GameState.Playing:
    //     break;
    // }
  }

  private void EnterState(GameState state) {
    Debug.Log("EnterState " + state);
    switch (state) {
      case GameState.Playing:
        menuPresenter.Clear();
        CursorManager.Instance.SetCursorLockState(true);
        timeManager.SetTimeScale(1);
        OnLevelChanged?.Invoke();
        break;
      case GameState.MainMenu:
        menuPresenter.ShowMainMenu();
        CursorManager.Instance.SetCursorLockState(false);
        timeManager.SetTimeScale(0);
        break;
      case GameState.Paused:
        menuPresenter.ShowPauseMenu();
        CursorManager.Instance.SetCursorLockState(false);
        timeManager.SetTimeScale(0);
        break;
      case GameState.GameOver:
        menuPresenter.ShowGameOver();
        CursorManager.Instance.SetCursorLockState(false);
        break;
      case GameState.LevelCompleted:
        Debug.Log($"Level {CurrentLevel} completed");
        CurrentLevel++;
        menuPresenter.ShowLevelCompleted();
        CursorManager.Instance.SetCursorLockState(false);
        break;
      case GameState.GameWon:
        menuPresenter.ShowGameWon();
        CursorManager.Instance.SetCursorLockState(false);
        break;
    }
  }

  private void InitLevels() {
    var levels = FindObjectsByType<Level>(FindObjectsSortMode.InstanceID);
    for (int i = 0; i < levels.Length; i++) {
      if (i < levels.Length - 1) {
        levels[i].OnLevelCompleted += NextLevel;
      } else {
        levels[i].OnLevelCompleted += GameWon;
      }
    }
    CurrentLevel = 1;
  }

  private void InitUICallbacks() {
    menuPresenter = FindFirstObjectByType<MenuUIPresenter>();
    menuPresenter.OnTryAgainClicked += HandleTryAgainClicked;
    menuPresenter.OnStartClicked += HandleStartClicked;
    menuPresenter.OnExitClicked += HandleExitClicked;
    menuPresenter.OnContinueClicked += HandleContinueClicked;
    menuPresenter.OnMainMenuClicked += HandleMainMenuClicked;
  }

  private async void NextLevel() {
    await UniTask.Delay(1000);
    ChangeState(GameState.LevelCompleted);
  }
  private async void GameWon() {
    await UniTask.Delay(1000);
    ChangeState(GameState.GameWon);
  }

  private async void HandlePlayerExploded() {
    await UniTask.Delay(1000);
    ChangeState(GameState.GameOver);
  }

  private void HandleStartClicked() {
    ChangeState(GameState.Playing);
  }

  private void HandleExitClicked() {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  private void HandleTryAgainClicked() {
    ChangeState(GameState.Playing);
    // TODO update to only replay current level
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  private void HandleContinueClicked() {
    ChangeState(GameState.Playing);
  }

  private void HandleMainMenuClicked() {
    ChangeState(GameState.MainMenu);
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
