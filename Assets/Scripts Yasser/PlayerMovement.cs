using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public float speeda;
        private float currentSpeed = 0f;      // Current speed of the player

    void Start()
    {
        // Get references to the NavMeshAgent and Animator components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Set the agent's stopping distance to a small value for a sharp stop
        agent.stoppingDistance = 0.1f;  // The agent stops when it gets this close to the target
        agent.angularSpeed = 500f;      // Set the turning speed high for sharp turning
        agent.acceleration = 10f;       // Set a higher acceleration for quick starts and stops
    }

    void Update()
    {
        // Handle movement based on mouse click
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
                RotateTowardsMouse(hit.point); // Rotate to the mouse position immediately
            }
        }

        // Update the 'Speed' parameter in the Animator to control the transition between Idle and Walking
        float speed = agent.velocity.magnitude * Time.deltaTime * 120; // Get the current speed of the player
        speeda = speed;
        animator.SetFloat("Speed", speed); // Update the Speed parameter in Animator
    }

    // Rotate the player to face the direction of the mouse position
    void RotateTowardsMouse(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0;  // Keep the rotation on the horizontal plane
        Quaternion toRotation = Quaternion.LookRotation(targetDirection);  // Create the rotation
        transform.rotation = toRotation;  // Apply the rotation directly
    }
}