using UnityEngine;

public class EnemySpawnArrow : MonoBehaviour {
  [SerializeField] private Transform spawnPoint;

  private GameObject enemyPrefab;

  public void SetEnemyPrefab(GameObject enemyPrefab) {
    this.enemyPrefab = enemyPrefab;
  }

  public void UpdateColor(MissileData missileData) {
    GetComponent<SpriteRenderer>().color = GameManager.GetMissileColor(missileData.color);
  }

  public void SpawnEnemy() {
    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
  }
}
