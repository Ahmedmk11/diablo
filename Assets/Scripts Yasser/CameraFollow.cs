using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // The player to follow
    public Vector3 positionOffset = new Vector3(0, 10, -10); // Adjusted offset for top-down view
    public float smoothSpeed = 5f;   // Speed for smooth camera movement
    public float rotationX = 60f;    // Fixed tilt angle for top-down view
    public float rotationY = 60f;

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position
            Vector3 desiredPosition = target.position + positionOffset;

            // Smoothly move the camera to the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

            // Set a fixed rotation for the top-down view
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
    }
}
