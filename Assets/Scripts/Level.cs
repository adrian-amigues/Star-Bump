using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour {
  public event Action OnLevelCompleted;

  private Dictionary<Damageable, bool> areEnemiesDeadMap = new Dictionary<Damageable, bool>();

  private void OnEnable() {
    InitLevel();
  }

  private void InitLevel() {

    var enemies = GetComponentsInChildren<Damageable>();

    foreach (var enemy in enemies) {
      areEnemiesDeadMap.Add(enemy, false);
      enemy.OnDeath += () => OnEnemyDeath(enemy);
    }
  }

  private void OnEnemyDeath(Damageable enemy) {
    areEnemiesDeadMap[enemy] = true;
    CheckAreAllEnemiesDead();
  }

  private void CheckAreAllEnemiesDead() {
    if (areEnemiesDeadMap.All(enemy => !!enemy.Value)) {
      Debug.Log("All enemies dead");
      OnLevelCompleted?.Invoke();
    }
  }
}
