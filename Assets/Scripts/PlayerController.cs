using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
  [SerializeField] private float moveSpeed = 4f;

  private readonly float xBound = 8f;
  private readonly float yBound = 4f;

  private InputSystem_Actions controls;
  private Vector2 moveInput;
  private Rigidbody2D rb;

  private void Awake() {
    rb = GetComponent<Rigidbody2D>();

    controls = new InputSystem_Actions();
    controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
  }

  private void FixedUpdate() {
    MovePlayer();
    FaceMouse();
  }

  private void OnEnable() {
    controls.Player.Enable();
  }

  private void OnDisable() {
    controls.Player.Disable();
  }

  private void MovePlayer() {
    var targetPos = rb.position + (moveInput.normalized * (moveSpeed * Time.fixedDeltaTime));
    var clampedX = Mathf.Clamp(targetPos.x, -xBound, xBound);
    var clampedY = Mathf.Clamp(targetPos.y, -yBound, yBound);

    rb.MovePosition(new Vector2(clampedX, clampedY));
  }

  private void FaceMouse() {
    var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    Vector2 direction = mousePos - transform.position;
    transform.up = direction;
  }
}
