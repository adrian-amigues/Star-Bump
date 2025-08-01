using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : Singleton<CursorManager> {
  [SerializeField] private Texture2D cursorTexture;

  private Vector2 originalHotspot = new Vector2(0, 0);
  private Texture2D transparentCursor;

  protected override void Awake() {
    base.Awake();
    CreateTransparentCursor();
    Cursor.SetCursor(cursorTexture, originalHotspot, CursorMode.Auto);
  }

  private void OnApplicationFocus(bool hasFocus) {
    if (hasFocus && GameManager.Instance.State == GameManager.GameState.Playing) {
      SetCursorLockState(true);
    }
  }

  private void OnDestroy() {
    if (transparentCursor != null) {
      Destroy(transparentCursor);
    }
  }

  private void CreateTransparentCursor() {
    transparentCursor = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    transparentCursor.SetPixel(0, 0, Color.clear);
    transparentCursor.Apply();
  }

  public void SetCursorLockState(bool shouldLock) {
    if (shouldLock) {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.SetCursor(transparentCursor, Vector2.zero, CursorMode.Auto);
      ClearCursorDelta();
    } else {
      Vector2 cursorHotspot = originalHotspot;
      Cursor.lockState = CursorLockMode.None;
      Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
  }

  public void ClearCursorDelta() {
    Mouse.current.delta.ReadValue(); // Clear any stale delta
  }
}
