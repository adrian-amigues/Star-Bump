using System;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
  // TODO: change that since we destroy it on try again, check which is best way to handle it
  [SerializeField] private GameObject player;

  public int CurrentLevel { get; private set; }

  public event Action OnLevelChanged;

  private MenuUIPresenter menuPresenter;

  protected override void Awake() {
    base.Awake();
    menuPresenter = FindFirstObjectByType<MenuUIPresenter>();
  }

  private void Start() {
    CurrentLevel = 1;
    player.GetComponentInChildren<PlayerDeathEffects>().OnPlayerExploded += HandlePlayerExploded;
    menuPresenter.OnTryAgainClicked += HandleTryAgainClicked;
  }

  public void NextLevel() {
    CurrentLevel++;
    OnLevelChanged?.Invoke();
  }

  private async void HandlePlayerExploded() {
    await UniTask.Delay(1000);
    menuPresenter.ShowGameOver();
  }

  private void HandleTryAgainClicked() {
    Destroy(player);
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
