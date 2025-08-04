using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
  [SerializeField] private float moveSpeed = 4f;

  [Header("Rotation Settings")]
  [SerializeField] private float rotationSensitivity = 0.1f;
  [SerializeField] private bool useTrackpadMode = false;
  [SerializeField] private float trackpadSensitivityMultiplier = 3f;
  [SerializeField] private float smoothTime = 0.1f;

  public Vector2 CurrentVelocity => currentVelocity;

  private Vector2 currentVelocity;
  private InputSystem_Actions controls;
  private Vector2 moveInput;
  private Rigidbody2D rb;
  private float shieldRotation = 0f;
  private float deathSlowDownDuration = 1f;
  private bool allowMovement;
  private float initialMoveSpeed;

  private void Awake() {
    rb = GetComponentInChildren<Rigidbody2D>();

    controls = new InputSystem_Actions();
    controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
  }

  private void Start() {
    allowMovement = true;
    initialMoveSpeed = moveSpeed;

    InitPlayerCallbacks(gameObject);
    GameManager.Instance.OnPlayerSpawned += InitPlayerCallbacks;
  }

  private void FixedUpdate() {
    if (!allowMovement) return;

    MovePlayer();
    RotateShieldsWithMouseDelta();
  }

  private void OnEnable() {
    controls.Player.Enable();
  }

  private void OnDisable() {
    controls.Player.Disable();
    GameManager.Instance.OnPlayerSpawned -= InitPlayerCallbacks;
  }

  private void OnDestroy() {
    // GameManager.Instance.OnPlayerSpawned -= InitPlayerCallbacks;
    // GetComponentInChildren<Damageable>().OnDeath -= HandlePlayerDeathStart;
    // GetComponent<PlayerDeathEffects>().OnPlayerExploded -= HandlePlayerExploded;
  }

  private void InitPlayerCallbacks(GameObject player) {
    player.GetComponentInChildren<Damageable>().OnDeath += HandlePlayerDeathStart;
    player.GetComponent<PlayerDeathEffects>().OnPlayerExploded += HandlePlayerExploded;
  }

  private void MovePlayer() {
    Vector2 targetVelocity = moveInput.normalized * moveSpeed;
    var currentPos = rb.position;

    var smoothedVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, smoothTime);

    Vector2 predictedNextPosition = currentPos + targetVelocity * Time.fixedDeltaTime;

    if (!PlayerMovementBounds.Instance.IsWithinBounds(predictedNextPosition)) {
      Vector2 clampedPosition = PlayerMovementBounds.Instance.ClampToBounds(predictedNextPosition);
      rb.linearVelocity = (clampedPosition - currentPos) / Time.fixedDeltaTime;
    } else {
      rb.linearVelocity = smoothedVelocity;
    }

  }

  private void RotateShieldsWithMouseDelta() {
    Vector2 mouseDelta = Mouse.current.delta.ReadValue();

    float sensitivity = rotationSensitivity;
    if (useTrackpadMode) {
      sensitivity *= trackpadSensitivityMultiplier;
    }

    shieldRotation += -mouseDelta.x * sensitivity;
    transform.rotation = Quaternion.Euler(0, 0, shieldRotation);
  }

  private void HandlePlayerDeathStart() {
    StartCoroutine(PlayerDeathSlowDownRoutine());
  }

  private IEnumerator PlayerDeathSlowDownRoutine() {
    float elapsed = 0f;
    float startSpeed = moveSpeed;

    while (elapsed < deathSlowDownDuration) {
      elapsed += Time.deltaTime;
      float progress = elapsed / deathSlowDownDuration;

      moveSpeed = Mathf.Lerp(startSpeed, 0f, progress);
      yield return null;
    }
  }

  private void HandlePlayerExploded() {
    allowMovement = false;

    var shipCollider = gameObject.GetComponentInChildren<PolygonCollider2D>();
    shipCollider.gameObject.SetActive(false);

    SetShieldsActive(false);
  }

  // public void RestorePlayerMovement() {
  //   allowMovement = true;
  //   moveSpeed = initialMoveSpeed;
  //   // var shipCollider = gameObject.GetComponentInChildren<PolygonCollider2D>();
  //   // shipCollider.gameObject.SetActive(true);

  //   ResetRotation();
  //   SetShieldsActive(true);
  // }

  private void SetShieldsActive(bool active) {
    var shields = gameObject.GetComponentsInChildren<PlayerShield>();
    foreach (var shield in shields) {
      shield.gameObject.SetActive(active);
    }
  }

  public void ResetRotation() {
    shieldRotation = 0f;
    // CursorManager.Instance.ClearCursorDelta();
  }
}
