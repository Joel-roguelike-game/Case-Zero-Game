using UnityEngine;

/*como se mueve la camara conforme se mueve el jugador por la sala*/
public class CameraFollow : MonoBehaviour
{
    public Transform target; // Player
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Sala Bounds")]
    public BoxCollider2D roomCollider;
    [Header("Padding de la cámara")]
    [SerializeField] public Vector2 camPadding = new Vector2(1f, 1f);


    private Camera cam;
    private float halfHeight;
    private float halfWidth;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    /*Inicializa la camara y asigna sus limites de movimiento */
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
    /*Actualiza la posicion */
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Clamp para que la cámara no vea más allá de la sala
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + halfWidth - camPadding.x, maxBounds.x - halfWidth + camPadding.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + halfHeight - camPadding.y, maxBounds.y - halfHeight + camPadding.y);

        transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);
    }
}