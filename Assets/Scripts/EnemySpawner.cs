using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System.Linq;
using UnityEngine.Playables;

public class EnemySpawner : MonoBehaviour {
  [SerializeField] private GameObject enemyPrefab;
  [SerializeField] private Transform spawnPoint;
  [SerializeField] private float spawnRate = 2f;
  [SerializeField] private float initialDelay = 1f;

  private float playerHitCastDistance = 10f;
  private LayerMask trajectoryStopLayers;
  private PlayableDirector timelineDirector;

  private void Awake() {
    trajectoryStopLayers = LayerMask.GetMask("Spawner", "TrajectoryStop");
    timelineDirector = GetComponent<PlayableDirector>();
  }

  private void Start() {
    InvokeRepeating("SpawnEnemy", initialDelay, spawnRate);
    GetComponent<Damageable>().OnDeath += HandleDeath;
  }

  private void SpawnEnemy() {
    if (!HasPlayerInView()) return;
    GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
  }

  private bool HasPlayerInView() {
    Vector2 direction = PlayerController.Instance.transform.position - spawnPoint.position;
    RaycastHit2D hit = Physics2D.Raycast(spawnPoint.position, direction, playerHitCastDistance, trajectoryStopLayers);
    Debug.Log($"Hit: {hit.collider}");
    Debug.Log($"Hit collider tag: {hit.collider?.gameObject.tag}");

    return hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("TrajectoryStop");
  }

  private void HandleDeath() {
    CancelInvoke("SpawnEnemy");
    // FadeOutAndDestroyRoutine().Forget();
    timelineDirector.Play();
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
