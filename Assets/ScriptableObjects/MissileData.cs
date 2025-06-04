using UnityEngine;

[CreateAssetMenu(fileName = "MissileData", menuName = "Scriptable Objects/Missile Data")]
public class MissileData : ScriptableObject {
  public MissileColor color;
  public Color visualColor;
  public GameObject destroyVfx;
  public float speed = 1f;
  public float damage = 1f;
}
