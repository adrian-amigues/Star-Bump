using System;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class GameManager : Singleton<GameManager> {
  [SerializeField] private GameObject playerPrefab;
  [SerializeField] public Transform playerSpawnPoint;
  [SerializeField] public ColorData mainColors;

  // public int CurrentLevel { get; private set; }
  public bool IsPlayerDead { get; private set; }
  public GameState State { get; private set; }

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
    LevelManager.Instance.OnGameWon += GameWon;
    LevelManager.Instance.OnLevelCompleted += HandleLevelCompleted;

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
    switch (state) {
      case GameState.GameOver:
        Debug.Log("ExitState GameOver");
        // player.RestorePlayerMovement();
        // playerDeathEffects.ResetPlayerVisuals();
        break;
    }
  }

  private void EnterState(GameState state) {
    Debug.Log("EnterState " + state);
    switch (state) {
      case GameState.Playing:
        menuPresenter.Clear();
        CursorManager.Instance.SetCursorLockState(true);
        timeManager.SetTimeScale(1);
        // LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
        break;
      case GameState.MainMenu:
        menuPresenter.ShowMainMenu();
        CursorManager.Instance.SetCursorLockState(false);
        timeManager.SetTimeScale(1);
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
        Debug.Log($"Level {LevelManager.Instance.CurrentLevel} completed");
        menuPresenter.ShowLevelCompleted();
        CursorManager.Instance.SetCursorLockState(false);
        break;
      case GameState.GameWon:
        menuPresenter.ShowGameWon();
        CursorManager.Instance.SetCursorLockState(false);
        break;
    }
  }

  private void InitUICallbacks() {
    menuPresenter = FindFirstObjectByType<MenuUIPresenter>();
    menuPresenter.OnTryAgainClicked += HandleTryAgainClicked;
    menuPresenter.OnStartClicked += HandleStartClicked;
    menuPresenter.OnExitClicked += HandleExitClicked;
    menuPresenter.OnContinueClicked += HandleContinueClicked;
    menuPresenter.OnNextLevelClicked += HandleNextLevelClicked;
    menuPresenter.OnMainMenuClicked += HandleMainMenuClicked;
  }

  private async void HandleLevelCompleted() {
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
    LevelManager.Instance.LoadLevel(1);
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
    SpawnPlayer();
    LevelManager.Instance.ResetCurrentLevel();
    ChangeState(GameState.Playing);
  }

  private void HandleContinueClicked() {
    ChangeState(GameState.Playing);
  }

  private void HandleNextLevelClicked() {
    LevelManager.Instance.NextLevel();
    ChangeState(GameState.Playing);
  }

  private void HandleMainMenuClicked() {
    // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    ChangeState(GameState.MainMenu);
    // LevelManager.Instance.LoadLevel(1);
  }

  // private void ResetPlayer() {
  //   Destroy(player.gameObject);
  //   SpawnPlayer();
  // }

  // private void InitPlayer() {
  //   player = FindFirstObjectByType<PlayerController>();
  //   playerDeathEffects = player.GetComponentInChildren<PlayerDeathEffects>();
  //   playerDeathEffects.OnPlayerExploded += HandlePlayerExploded;
  //   IsPlayerDead = false;
  // }

  private void SpawnPlayer() {
    var player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
    // cinemachine follow target
    var cinemachineFollow = FindFirstObjectByType<CinemachineCamera>();
    if (cinemachineFollow != null) {
      cinemachineFollow.Follow = player.transform;
    }
  }

  public bool TryGetPlayer(out Transform playerTransform) {
    var player = FindFirstObjectByType<PlayerController>();
    if (player == null) {
      playerTransform = null;
      return false;
    }
    playerTransform = player.transform;
    return true;
  }

  public static Color GetMissileColor(MissileColor color) {
#if UNITY_EDITOR
    if (!Application.isPlaying) {
      var colorData = AssetDatabase.LoadAssetAtPath<ColorData>("Assets/ScriptableObjects/ColorData/MainColors.asset");
      if (!colorData) {
        Debug.LogError("ColorData not found");
        return Color.white;
      }
      return colorData.GetColor(color);
    }
#endif
    return Instance.mainColors?.GetColor(color) ?? Color.white;
  }
}
