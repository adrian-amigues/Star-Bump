using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System.Linq;
using UnityEngine.Playables;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour {
  [SerializeField] private GameObject enemyPrefab;
  [SerializeField] private GameObject spawnArrow;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private float spawnRate = 2f;
  [SerializeField] private float initialDelay = 1f;

  private float playerHitCastDistance = 20f;
  private LayerMask trajectoryStopLayers;
  private PlayableDirector timelineDirector;
  private float minDistanceToPlayer = 1f;

  private void OnValidate() {
    if (Application.isPlaying || !enemyPrefab || !spawnArrow) return;

    var missileData = enemyPrefab.GetComponent<EnemyMissile>()?.MissileData;
    if (!missileData) return;
    spawnArrow.GetComponent<SpriteRenderer>().color = GameManager.GetMissileColor(missileData.color);
  }

  private void Awake() {
    trajectoryStopLayers = LayerMask.GetMask("Spawner", "TrajectoryStop");
    timelineDirector = GetComponent<PlayableDirector>();
  }

  private void OnEnable() {
    InvokeRepeating("SpawnEnemy", initialDelay, spawnRate);
    GetComponent<Damageable>().OnDeath += HandleDeath;
  }

  private void OnDisable() {
    CancelInvoke("SpawnEnemy");
    GetComponent<Damageable>().OnDeath -= HandleDeath;
  }

  private void SpawnEnemy() {
    if (!HasPlayerInView()) return;
    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
  }

  private bool HasPlayerInView() {
    if (!GameManager.Instance.TryGetPlayer(out var player)) return false;

    Vector2 direction = player.position - spawnPoint.position;

    var castOrigin = spawnPoint.position + (Vector3)direction.normalized * minDistanceToPlayer;
    Debug.Log($"Cast origin: {castOrigin}");
    RaycastHit2D hit = Physics2D.CircleCast(castOrigin, 0.5f, direction, playerHitCastDistance, trajectoryStopLayers);

    return hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("TrajectoryStop");
  }

  private void HandleDeath() {
    CancelInvoke("SpawnEnemy");
    DisableAllColliders();
    timelineDirector.Play();
  }

  private void DisableAllColliders() {
    var colliders = GetComponentsInChildren<Collider2D>();
    foreach (var collider in colliders) {
      collider.enabled = false;
    }
  }

  public void DestroySpawner() {
    Destroy(gameObject);
  }
}
