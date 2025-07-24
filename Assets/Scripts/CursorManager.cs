using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : Singleton<CursorManager> {
  protected override void Awake() {
    base.Awake();

    // Todo: update when starting on Main Menu
    SetCursorLockState(true);
  }

  private void Update() {
    if (Keyboard.current.escapeKey.wasPressedThisFrame) {
      SetCursorLockState(!Cursor.visible);
    }
  }

  private void OnApplicationFocus(bool hasFocus) {
    if (hasFocus) {
      SetCursorLockState(true);
    }
  }

  public void SetCursorLockState(bool isLocked) {
    Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !isLocked;

    if (isLocked) {
      Mouse.current.delta.ReadValue(); // Clear any stale delta
    }
  }
}
