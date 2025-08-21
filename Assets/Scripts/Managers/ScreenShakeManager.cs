using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;

// Unused for now
public class ScreenShakeManager : Singleton<ScreenShakeManager> {
  private CinemachineImpulseSource impulseSource;

  protected override void Awake() {
    base.Awake();
    impulseSource = GetComponent<CinemachineImpulseSource>();
  }

  public void ShakeScreen() {
    // impulseSource.GenerateImpulse();
    Tween.ShakeCamera(Camera.main, strengthFactor: 0.5f);
  }
}