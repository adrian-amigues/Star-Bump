using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController> {
  [SerializeField] private float moveSpeed = 4f;

  [Header("Rotation Settings")]
  [SerializeField] private float rotationSensitivity = 0.1f;
  [SerializeField] private bool useTrackpadMode = false;
  [SerializeField] private float trackpadSensitivityMultiplier = 3f;

  public Vector2 CurrentVelocity { get; private set; }

  private InputSystem_Actions controls;
  private Vector2 moveInput;
  private Rigidbody2D rb;
  private float shieldRotation = 0f;
  private Vector2 lastPosition;
  private bool isPlayerDead = false;
  private float deathSlowDownDuration = 1f;

  protected override void Awake() {
    base.Awake();
    rb = GetComponent<Rigidbody2D>();

    SetCursorLockState(true);

    controls = new InputSystem_Actions();
    controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
  }

  private void Start() {
    GetComponent<Damageable>().OnDeath += HandlePlayerDeathStart;
    GetComponent<PlayerDeathEffects>().OnPlayerExploded += HandlePlayerExploded;
  }

  private void Update() {
    if (Keyboard.current.escapeKey.wasPressedThisFrame) {
      SetCursorLockState(!Cursor.visible);
    }
  }

  private void FixedUpdate() {
    if (isPlayerDead) return;

    MovePlayer();
    RotateShieldsWithMouseDelta();
    UpdateVelocityTracking();
  }

  private void OnEnable() {
    controls.Player.Enable();
  }

  private void OnDisable() {
    controls.Player.Disable();
  }

  private void OnApplicationFocus(bool hasFocus) {
    if (hasFocus) {
      SetCursorLockState(true);
    }
  }

  private void UpdateVelocityTracking() {
    CurrentVelocity = (rb.position - lastPosition) / Time.fixedDeltaTime;
    lastPosition = rb.position;
  }

  private void SetCursorLockState(bool isLocked) {
    Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !isLocked;

    if (isLocked) {
      Mouse.current.delta.ReadValue(); // Clear any stale delta
    }
  }

  private void MovePlayer() {
    var targetPos = rb.position + (moveInput.normalized * (moveSpeed * Time.fixedDeltaTime));

    targetPos = PlayerMovementBounds.Instance.ClampToBounds(targetPos);

    rb.MovePosition(targetPos);
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
    isPlayerDead = true;

    var shipCollider = gameObject.GetComponentInChildren<PolygonCollider2D>();
    shipCollider.gameObject.SetActive(false);

    var shields = gameObject.GetComponentsInChildren<PlayerShield>();
    foreach (var shield in shields) {
      shield.gameObject.SetActive(false);
    }
  }
}
