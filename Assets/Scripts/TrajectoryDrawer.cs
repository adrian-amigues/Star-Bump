using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MissileTrajectoryDrawer : MonoBehaviour {
  [SerializeField] private float segmentLength = 5f;
  [SerializeField] private LayerMask bounceLayers;
  [SerializeField] private int maxBounces = 1;
  [SerializeField] private GameObject impactShadowPrefab;

  private LineRenderer line;
  private Rigidbody2D rb;
  private MissileColor missileColor;
  private float sourceObjectRadius;
  private GameObject currentImpactShadow;

  private void Awake() {
    line = GetComponent<LineRenderer>();
    rb = GetComponent<Rigidbody2D>();
    InitializeRadius();
  }

  private void Start() {
    // Important to have this here in Start() instead of Awake() because the EnemyMissile component might come from an assignation after instantiation like in EnemySpawner
    missileColor = GetComponent<EnemyMissile>().MissileData.color;
  }

  private void InitializeRadius() {
    CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
    if (circleCollider != null) {
      sourceObjectRadius = circleCollider.radius * transform.localScale.x; // Account for scale
    } else {
      sourceObjectRadius = 0.5f; // Fallback default radius
    }
  }

  public void DrawTrajectory() {
    Vector2 startPos = transform.position;
    Vector2 direction = rb.linearVelocity.normalized;

    line.positionCount = 1;
    line.SetPosition(0, startPos);

    Vector2 currentPos = startPos;
    Vector2 currentDir = direction;
    int bounces = 0;

    while (bounces <= maxBounces) {
      // Use CircleCast to match the actual physics collision
      RaycastHit2D hit = Physics2D.CircleCast(currentPos, sourceObjectRadius, currentDir, segmentLength, bounceLayers);

      if (hit.collider != null) {
        line.positionCount += 1;
        bounces++;

        if (hit.collider.TryGetComponent(out PlayerShield shield) && shield.shieldColor == missileColor) {
          Vector2 contactNormal = hit.normal;

          // Position the line at the circle center when touching
          Vector2 circleCenterAtHit = hit.point + contactNormal * sourceObjectRadius;
          line.SetPosition(line.positionCount - 1, circleCenterAtHit);
          ShowImpactShadow(circleCenterAtHit);

          currentPos = hit.point + contactNormal * (sourceObjectRadius + 0.01f);
          currentDir = Vector2.Reflect(currentDir, contactNormal);
        } else {
          line.SetPosition(line.positionCount - 1, hit.point);
        }
      } else {
        if (bounces > 0) {
          // draw last straight segment
          Vector2 end = currentPos + currentDir * segmentLength;
          line.positionCount += 1;
          line.SetPosition(line.positionCount - 1, end);
        } else {
          HideImpactShadow();
        }
        break;
      }
    }
  }

  public void HideTrajectory() {
    line.positionCount = 0;
    HideImpactShadow();
  }

  private void ShowImpactShadow(Vector2 position) {
    if (currentImpactShadow == null) {
      currentImpactShadow = Instantiate(impactShadowPrefab);
    }
    currentImpactShadow.transform.position = position;
    currentImpactShadow.SetActive(true);
  }

  private void HideImpactShadow() {
    if (currentImpactShadow != null) {
      currentImpactShadow.SetActive(false);
    }
  }

  private void OnDestroy() {
    if (currentImpactShadow != null) {
      Destroy(currentImpactShadow);
    }
  }
}
