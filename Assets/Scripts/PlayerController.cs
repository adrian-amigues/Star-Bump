using UnityEngine;

public class PlayerController : MonoBehaviour {
  [SerializeField] private float moveSpeed = 4f;

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
  }

  private void OnEnable() {
    controls.Player.Enable();
  }

  private void OnDisable() {
    controls.Player.Disable();
  }

  private void MovePlayer() {
    var targetPos = rb.position + (moveInput.normalized * (moveSpeed * Time.fixedDeltaTime));
    rb.MovePosition(targetPos);
  }
}
