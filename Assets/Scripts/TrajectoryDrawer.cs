using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MissileTrajectoryDrawer : MonoBehaviour {
  [SerializeField] private float segmentLength = 5f;
  [SerializeField] private LayerMask bounceLayers;
  [SerializeField] private int maxBounces = 1;

  private LineRenderer line;
  private Rigidbody2D rb;
  private MissileColor missileColor;
  private float sourceObjectRadius;

  private void Awake() {
    line = GetComponent<LineRenderer>();
    rb = GetComponent<Rigidbody2D>();
    InitializeRadius();
  }

  private void Start() {
    // Important to have this here in Start() instead of Awake() because the EnemyMissile component might come from an assignation after instantiation like in EnemySpawner
    missileColor = GetComponent<EnemyMissile>().missileData.color;
  }

  private void FixedUpdate() {
    DrawTrajectory();
  }

  private void InitializeRadius() {
    CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
    if (circleCollider != null) {
      sourceObjectRadius = circleCollider.radius * transform.localScale.x; // Account for scale
    } else {
      sourceObjectRadius = 0.5f; // Fallback default radius
    }
  }

  private void DrawTrajectory() {
    Vector2 startPos = transform.position;
    Vector2 direction = rb.linearVelocity.normalized;

    line.positionCount = 1;
    line.SetPosition(0, startPos);

    Vector2 currentPos = startPos;
    Vector2 currentDir = direction;
    int bounces = 0;

    while (bounces <= maxBounces) {
      // Use CircleCast instead of Raycast to account for missile radius
      RaycastHit2D hit = Physics2D.CircleCast(currentPos, sourceObjectRadius, currentDir, segmentLength, bounceLayers);

      if (hit.collider != null) {
        line.positionCount += 1;
        line.SetPosition(line.positionCount - 1, hit.point);
        bounces++;

        if (hit.collider.TryGetComponent(out PlayerShield shield) && shield.shieldColor == missileColor) {
          // Move the current position to the hit point, offset by radius in the normal direction
          currentPos = hit.point + hit.normal * (sourceObjectRadius + 0.01f);
          currentDir = Vector2.Reflect(currentDir, hit.normal);
        }
      } else {
        // draw last straight segment
        Vector2 end = currentPos + currentDir * segmentLength;
        line.positionCount += 1;
        line.SetPosition(line.positionCount - 1, end);
        break;
      }
    }
  }

  // private void DrawTrajectoryWithGravityTakenIntoAccount() {
  //   List<Vector2> points = new List<Vector2>(); // Store all points for the LineRenderer

  //   Vector2 currentPos = missileRigidbody.position; // Use Rigidbody.position
  //   Vector2 currentVel = missileRigidbody.velocity; // Use Rigidbody.velocity

  //   points.Add(currentPos); // Add the starting point

  //   int bounces = 0;
  //   float timeElapsed = 0f;

  //   while (timeElapsed < maxPredictionTime && bounces <= maxBounces) // Predict until max time or max bounces
  //   {
  //     // 1. Calculate next potential position
  //     Vector2 nextVel = currentVel + currentGravity * timeStep;
  //     Vector2 nextPos = currentPos + (currentVel + nextVel) * 0.5f * timeStep; // More accurate integration (midpoint)

  //     // 2. Check for collision between currentPos and nextPos
  //     RaycastHit2D hit = Physics2D.Linecast(currentPos, nextPos, bounceLayers);

  //     if (hit.collider != null) {
  //       // Collision detected!
  //       points.Add(hit.point); // Add the exact hit point

  //       // Calculate reflected velocity
  //       Vector2 incidentVelocity = nextVel; // Or currentVel, depending on accuracy needs
  //       Vector2 reflectedVelocity = Vector2.Reflect(incidentVelocity, hit.normal);

  //       // Apply dampening
  //       reflectedVelocity *= bounceDampening;

  //       // Update for the next segment
  //       currentPos = hit.point + hit.normal * 0.01f; // Offset slightly
  //       currentVel = reflectedVelocity;
  //       bounces++;

  //       // Reset time for the new segment (or recalculate time remaining to hit point)
  //       // For simplicity, we'll just continue the loop with the new velocity
  //     } else {
  //       // No collision in this time step, continue along the curve
  //       points.Add(nextPos);
  //       currentPos = nextPos;
  //       currentVel = nextVel;
  //     }

  //     timeElapsed += timeStep;
  //   }

  //   // Update the LineRenderer
  //   line.positionCount = points.Count;
  //   line.SetPositions(points.ToArray());
  // }
}
