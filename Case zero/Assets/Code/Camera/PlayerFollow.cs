using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // Jugador
    public float smoothSpeed = 0.15f; // 0 = cámara dura, 1 = cámara suave
    public Vector3 offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        // Interpolación suave 
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothed;
    }
}