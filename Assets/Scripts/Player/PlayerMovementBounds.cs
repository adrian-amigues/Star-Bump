using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerMovementBounds : Singleton<PlayerMovementBounds> {
  private CircleCollider2D boundaryCollider;

  public Vector2 Center => boundaryCollider.bounds.center;
  public float Radius => boundaryCollider.radius;

  protected override void Awake() {
    base.Awake();
    boundaryCollider = GetComponent<CircleCollider2D>();
    boundaryCollider.isTrigger = true;
  }


  public Vector2 ClampToBounds(Vector2 position) {
    var direction = position - Center;
    return Center + Vector2.ClampMagnitude(direction, Radius);
  }

  public bool IsWithinBounds(Vector2 position) {
    return Vector2.Distance(position, Center) <= Radius;
  }
}
