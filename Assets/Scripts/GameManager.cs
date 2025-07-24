using System;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
  public int CurrentLevel { get; private set; }

  public event Action OnLevelChanged;

  protected override void Awake() {
    base.Awake();
  }

  private void Start() {
    CurrentLevel = 1;
  }

  public void NextLevel() {
    CurrentLevel++;
    OnLevelChanged?.Invoke();
  }
}
