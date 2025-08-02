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
    RaycastHit2D hit = Physics2D.CircleCast(castOrigin, 0.5f, direction, playerHitCastDistance, trajectoryStopLayers);

    return hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("TrajectoryStop");
  }

  private void HandleDeath() {
    CancelInvoke("SpawnEnemy");
    DisableAllColliders();
    timelineDirector.Play();
    // FadeOutAndDestroyRoutine().Forget();
  }

  private void DisableAllColliders() {
    var colliders = GetComponentsInChildren<Collider2D>();
    foreach (var collider in colliders) {
      collider.enabled = false;
    }
  }

  // private async UniTaskVoid FadeOutAndDestroyRoutine() {
  //   var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
  //   var originalColors = spriteRenderers.Select(sr => sr.color).ToArray();

  //   float elapsed = 0f;
  // while (elapsed < fadeOutDuration) {
  //   elapsed += Time.deltaTime;
  //   float progress = elapsed / fadeOutDuration;

  //   for (int i = 0; i < spriteRenderers.Length; i++) {
  //     var color = originalColors[i];
  //     color.a = Mathf.Lerp(1f, 0f, progress);
  //     spriteRenderers[i].color = color;
  //   }
  //   await UniTask.NextFrame();
  // }
  //   Destroy(gameObject);
  // }

  public void DestroySpawner() {
    Destroy(gameObject);
  }
}
