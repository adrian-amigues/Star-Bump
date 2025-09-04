using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CursorManager : Singleton<CursorManager> {
  [SerializeField] private Texture2D cursorTexture;

  private Vector2 originalHotspot = new Vector2(0, 0);
  private Texture2D transparentCursor;
  private bool pendingLockRequest = false;
  private Coroutine lockRetryCoroutine;

  protected override void Awake() {
    base.Awake();
    CreateTransparentCursor();
    Cursor.SetCursor(cursorTexture, originalHotspot, CursorMode.Auto);
  }

  private void Update() {
    // Handle WebGL cursor lock issues
#if UNITY_WEBGL && !UNITY_EDITOR
    HandleWebGLCursorLock();
#endif
  }

  private void OnDestroy() {
    if (transparentCursor != null) {
      Destroy(transparentCursor);
    }
    if (lockRetryCoroutine != null) {
      StopCoroutine(lockRetryCoroutine);
    }
  }

  private void OnApplicationFocus(bool hasFocus) {
    if (hasFocus && GameManager.Instance.State == GameManager.GameState.Playing) {
      SetCursorLockState(true);
    }
  }

  private void CreateTransparentCursor() {
    transparentCursor = new Texture2D(1, 1, TextureFormat.RGBA32, false);
    transparentCursor.SetPixel(0, 0, Color.clear);
    transparentCursor.Apply();
  }

#if UNITY_WEBGL && !UNITY_EDITOR
  private void HandleWebGLCursorLock() {
    // Check if we have a pending lock request and user clicked
    if (pendingLockRequest && Input.GetMouseButtonDown(0)) {
      SetCursorLockState(true);
      pendingLockRequest = false;
    }

    // Check if cursor unexpectedly unlocked during gameplay
    if (GameManager.Instance.State == GameManager.GameState.Playing &&
        Cursor.lockState != CursorLockMode.Locked) {
      pendingLockRequest = true;
    }
  }
#endif

  public void SetCursorLockState(bool shouldLock) {
    if (shouldLock) {
#if UNITY_WEBGL && !UNITY_EDITOR
      // WebGL requires user interaction to lock cursor
      if (lockRetryCoroutine != null) {
        StopCoroutine(lockRetryCoroutine);
      }
      lockRetryCoroutine = StartCoroutine(TryLockCursorWebGL());
#else
      LockCursor();
#endif
    } else {
      UnlockCursor();
      pendingLockRequest = false;
      if (lockRetryCoroutine != null) {
        StopCoroutine(lockRetryCoroutine);
        lockRetryCoroutine = null;
      }
    }
  }

#if UNITY_WEBGL && !UNITY_EDITOR
  private IEnumerator TryLockCursorWebGL() {
    int attempts = 0;
    const int maxAttempts = 10;

    while (attempts < maxAttempts && Cursor.lockState != CursorLockMode.Locked) {
      LockCursor();
      yield return new WaitForSeconds(0.1f);
      attempts++;
    }

    if (Cursor.lockState != CursorLockMode.Locked) {
      pendingLockRequest = true;
      Debug.Log("Click on the game window to lock cursor");
    }

    lockRetryCoroutine = null;
  }
#endif

  private void LockCursor() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.SetCursor(transparentCursor, Vector2.zero, CursorMode.Auto);
    ClearCursorDelta();
  }

  private void UnlockCursor() {
    Vector2 cursorHotspot = originalHotspot;
    Cursor.lockState = CursorLockMode.None;
    Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
  }

  public void ClearCursorDelta() {
    Mouse.current?.delta.ReadValue(); // Clear any stale delta
  }
}
