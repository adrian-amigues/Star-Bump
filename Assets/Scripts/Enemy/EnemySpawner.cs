using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

public class EnemySpawner : MonoBehaviour {
  // [SerializeField] private GameObject enemyPrefab;
  // [SerializeField] private GameObject spawnArrow;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private float spawnRate = 2f;
  [SerializeField] private float initialDelay = 1f;
  [SerializeField] private GameObject[] enemyPrefabs;

  private EnemySpawnArrow[] enemySpawnArrows;
  private float playerHitCastDistance = 20f;
  private LayerMask trajectoryStopLayers;
  private PlayableDirector timelineDirector;
  private float minDistanceToPlayer = 1f;
  private CancellationTokenSource cancellationTokenSource;

  private void OnValidate() {
    if (Application.isPlaying || enemyPrefabs.Length == 0) return;
    enemySpawnArrows = GetComponentsInChildren<EnemySpawnArrow>();

    for (int i = 0; i < enemyPrefabs.Length; i++) {
      var enemyPrefab = enemyPrefabs[i];
      var missileData = enemyPrefab.GetComponent<EnemyMissile>()?.MissileData;
      if (!missileData) return;
      enemySpawnArrows[i].UpdateColor(missileData);
    }
  }

  private void Awake() {
    trajectoryStopLayers = LayerMask.GetMask("Spawner", "TrajectoryStop");
    timelineDirector = GetComponent<PlayableDirector>();
  }

  private void OnEnable() {
    InitializeEnemySpawnArrows();
    StartSpawning();
    GetComponent<Damageable>().OnDeath += HandleDeath;
  }

  private void OnDisable() {
    StopSpawning();
    GetComponent<Damageable>().OnDeath -= HandleDeath;
  }

  private void Start() {
    InitSplineMovement();
  }

  private void InitializeEnemySpawnArrows() {
    enemySpawnArrows = GetComponentsInChildren<EnemySpawnArrow>();
    for (int i = 0; i < enemyPrefabs.Length; i++) {
      enemySpawnArrows[i].SetEnemyPrefab(enemyPrefabs[i]);
    }
  }

  // private void SpawnEnemy() {
  //   if (!HasPlayerInView()) return;
  //   foreach (var spawnArrow in enemySpawnArrows) {
  //     spawnArrow.SpawnEnemy();
  //   }
  // }

  private void StartSpawning() {
    StopSpawning();
    cancellationTokenSource = new CancellationTokenSource();
    SpawnEnemiesLoop(cancellationTokenSource.Token);
  }

  private void StopSpawning() {
    cancellationTokenSource?.Cancel();
    cancellationTokenSource?.Dispose();
    cancellationTokenSource = null;
  }

  private async void SpawnEnemiesLoop(CancellationToken cancellationToken) {
    try {
      await UniTask.Delay(TimeSpan.FromSeconds(initialDelay), cancellationToken: cancellationToken);

      while (!cancellationToken.IsCancellationRequested) {
        if (HasPlayerInView()) {
          await SpawnEnemiesFromPoints(cancellationToken);
        } else {
          await UniTask.Delay(TimeSpan.FromSeconds(spawnRate), cancellationToken: cancellationToken);
        }
      }
    } catch (OperationCanceledException) {
      return;
    }
  }

  private async UniTask SpawnEnemiesFromPoints(CancellationToken cancellationToken) {
    foreach (var spawnArrow in enemySpawnArrows) {
      spawnArrow.SpawnEnemy();
      await UniTask.Delay(TimeSpan.FromSeconds(spawnRate), cancellationToken: cancellationToken);
    }
  }

  private bool HasPlayerInView() {
    if (!GameManager.Instance.TryGetPlayer(out var player)) return false;

    Vector2 direction = player.position - spawnPoint.position;

    var castOrigin = spawnPoint.position + (Vector3)direction.normalized * minDistanceToPlayer;
    RaycastHit2D hit = Physics2D.CircleCast(castOrigin, 0.5f, direction, playerHitCastDistance, trajectoryStopLayers);

    return hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("TrajectoryStop");
  }

  private void HandleDeath() {
    StopSpawning();
    DisableAllColliders();
    timelineDirector.Play();
  }

  private void DisableAllColliders() {
    var colliders = GetComponentsInChildren<Collider2D>();
    foreach (var collider in colliders) {
      collider.enabled = false;
    }
  }

  private void InitSplineMovement() {
    if (!TryGetComponent<SplineAnimate>(out var splineAnimate)) return;
    splineAnimate.NormalizedTime = 0.5f;
  }

  public void DestroySpawner() {
    Destroy(gameObject);
  }
}
