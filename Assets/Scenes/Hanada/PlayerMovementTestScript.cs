using UnityEngine;

public class PlayerMovementTestScript : MonoBehaviour
{
    private float moveSpeed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}