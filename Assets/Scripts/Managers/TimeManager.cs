using UnityEngine;

public class TimeManager : MonoBehaviour {
  public float TimeScale { get; private set; }

  private float previousTimeScale;

  void Update() {
    // Debug.Log("Time scale: " + Time.timeScale);
    // if (Time.timeScale != previousTimeScale) {
    //   Debug.Log("New time scale: " + Time.timeScale + ", previous: " + previousTimeScale);
    //   previousTimeScale = Time.timeScale;
    // }
  }

  public void SetTimeScale(float timeScale) {
    // Debug.Log("SetTimeScale " + timeScale);
    // previousTimeScale = Time.timeScale;
    Time.timeScale = timeScale;
  }
}
