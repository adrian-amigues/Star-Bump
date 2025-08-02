using UnityEngine;
using System;
using System.Linq;

public class LevelManager : Singleton<LevelManager> {
  [SerializeField] private int initialLevel = 1;
  [SerializeField] private GameObject[] levelPrefabs;
  [SerializeField] private GameObject exampleLevelsContainer;
  [SerializeField] private Transform levelContainer;

  public int CurrentLevel { get; private set; }
  public event Action OnGameWon;
  public event Action OnLevelCompleted;
  public event Action OnLevelChanged;

  // private Level[] levels;
  // private int previousLevel;
  private GameObject currentLevelInstance;

  private void Start() {
    CurrentLevel = initialLevel;
    DisableExampleLevels();
    // Init();
  }

  private void DisableExampleLevels() {
    exampleLevelsContainer.SetActive(false);
  }

  public void ResetPlayerPosition() {
    var player = FindFirstObjectByType<PlayerController>();
    player.transform.position = GameManager.Instance.playerSpawnPoint.position;
    player.ResetRotation();

    var playerRigidbody = player.GetComponent<Rigidbody2D>();
    playerRigidbody.linearVelocity = Vector2.zero;
    playerRigidbody.angularVelocity = 0;


    // center camera on player
    // var camera = Camera.main;
    // camera.transform.position = player.transform.position;
    Debug.Log("player rotation2: " + player.transform.rotation);
  }

  public void LoadLevel(int level) {
    var prefabIndex = level - 1;
    if (prefabIndex >= 0 && prefabIndex < levelPrefabs.Length) {
      if (currentLevelInstance) {
        Destroy(currentLevelInstance);
      }

      currentLevelInstance = Instantiate(levelPrefabs[prefabIndex], levelContainer);
      currentLevelInstance.SetActive(true);

      var levelScript = currentLevelInstance.GetComponent<Level>();
      if (level == levelPrefabs.Length) {
        levelScript.OnLevelCompleted += () => OnGameWon?.Invoke();
      } else {
        levelScript.OnLevelCompleted += () => OnLevelCompleted?.Invoke();
      }

      CurrentLevel = level;
      ResetPlayerPosition();
      OnLevelChanged?.Invoke();
    } else {
      Debug.LogError($"Level {level} prefab not found");
    }
  }

  public void NextLevel() {
    LoadLevel(CurrentLevel + 1);
  }

  public void ResetCurrentLevel() {
    LoadLevel(CurrentLevel);
  }
}
