using UnityEngine;

[CreateAssetMenu(fileName = "MissileData", menuName = "Scriptable Objects/Missile Data")]
public class MissileData : ScriptableObject {
  public MissileColor color;
  public GameObject destroyVfx;
  public float speed = 1f;
  public float maxSpeed = 10f;
  [Range(0f, 50f)]
  public float acceleration = 5f;
  public int damage = 1;
}
