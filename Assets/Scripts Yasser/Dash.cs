using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dash : MonoBehaviour
{
    private Animator animator;
   
    private RaycastHit hit;
    public Camera camera;
    private NavMeshAgent agent;
    
    
    public Vector3 position;

    private bool isCooldown = false; // Cooldown flag
    private float cooldownTime = 10f; // Cooldown duration
    private float dashDelay = 2.3f; // Delay before executing the dash

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            // Create a ray from the camera to the mouse position
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) // Check if the ray hit something
            {
                
                position = hit.point;

                // Calculate direction ignoring the Y-axis for a horizontal rotation
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y; // Keep the player level on the X-Z plane

                // Make the player look at the target position
                agent.transform.LookAt(targetPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.D) && !isCooldown)
        {
            animator.SetTrigger("Dash");
            StartCoroutine(ExecuteDashWithDelay(position));
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator ExecuteDashWithDelay(Vector3 position)
    {
        // Wait for the specified dash delay
        yield return new WaitForSeconds(dashDelay);

        // Execute the dash function
        DashAb(position);
    }

    void DashAb(Vector3 position)
    {
        agent.transform.position = position;
    }

    IEnumerator Cooldown()
    {
        // Set cooldown flag
        isCooldown = true;

        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);

        // Reset cooldown flag
        isCooldown = false;
    }
}
