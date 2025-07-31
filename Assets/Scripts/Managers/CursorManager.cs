using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : Singleton<CursorManager> {
  [SerializeField] private Texture2D cursorTexture;

  private Vector2 originalHotspot = new Vector2(0, 0);
  // private Vector2 originalHotspot = new Vector2(20, 23);
  // private float originalTextureSize = 160f;
  private Texture2D transparentCursor;

  protected override void Awake() {
    base.Awake();
    CreateTransparentCursor();
    // Vector2 cursorHotspot = CalculateRealHotspot();
    Cursor.SetCursor(cursorTexture, originalHotspot, CursorMode.Auto);
  }

  // private void Update() {
  //   if (Keyboard.current.escapeKey.wasPressedThisFrame) {
  //     SetCursorLockState(!Cursor.visible);
  //   }
  // }

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

  // private Vector2 CalculateRealHotspot() {
  //   float scaleX = cursorTexture.width / originalTextureSize;
  //   float scaleY = cursorTexture.height / originalTextureSize;
  //   Vector2 scaledHotspot = new Vector2(originalHotspot.x * scaleX, originalHotspot.y * scaleY);
  //   return scaledHotspot;
  // }

  private void CreateTransparentCursor() {
    transparentCursor = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    transparentCursor.SetPixel(0, 0, Color.clear);
    transparentCursor.Apply();
  }

  public void SetCursorLockState(bool shouldLock) {
    Debug.Log("SetCursorLockState: " + shouldLock);
    if (shouldLock) {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.SetCursor(transparentCursor, Vector2.zero, CursorMode.Auto);
      Mouse.current.delta.ReadValue(); // Clear any stale delta
    } else {
      Vector2 cursorHotspot = originalHotspot;
      Cursor.lockState = CursorLockMode.None;
      Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
  }
}
