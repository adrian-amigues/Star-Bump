using System.Collections.Generic;
using UnityEngine;

// TODO: delete ?
[CreateAssetMenu(fileName = "ColorData", menuName = "Scriptable Objects/ColorData")]
public class ColorData : ScriptableObject {
  [Header("Missile Colors")]
  public Color blueColor;
  public Color pinkColor;

  public Color GetColor(MissileColor missileColor) {
    return missileColor switch {
      MissileColor.Blue => blueColor,
      MissileColor.Pink => pinkColor,
      _ => Color.white
    };
  }
}
