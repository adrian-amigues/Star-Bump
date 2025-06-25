using UnityEngine;

public class EnemySpawner : MonoBehaviour {
  [SerializeField] private GameObject enemyPrefab;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private MissileData missileData;
  [SerializeField] private float spawnRate = 2f;
  [SerializeField] private float initialDelay = 1f;

  private void Start() {
    InvokeRepeating("SpawnEnemy", initialDelay, spawnRate);
  }

  private void SpawnEnemy() {
    GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    EnemyMissile enemyMissile = enemyInstance.GetComponent<EnemyMissile>();
    enemyMissile.Initialize(missileData);
  }
}
