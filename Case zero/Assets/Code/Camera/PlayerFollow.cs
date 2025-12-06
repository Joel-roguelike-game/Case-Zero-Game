using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Jugador
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Sala Bounds")]
    public BoxCollider2D roomCollider; // Arrastra aquí el empty Limits de la sala

    private Camera cam;
    private float halfHeight;
    private float halfWidth;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = cam.aspect * halfHeight;

        if (roomCollider != null)
        {
            minBounds = roomCollider.bounds.min;
            maxBounds = roomCollider.bounds.max;
        }
        else
        {
            Debug.LogWarning("No hay collider de sala asignado a CameraFollow");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Clamp para que la cámara no vea más allá de la sala
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);
    }
}