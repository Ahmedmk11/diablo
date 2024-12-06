using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArrowShower : MonoBehaviour
{
    private Animator animator;
    public GameObject arrowPrefab; // Assign your arrow prefab
    private RaycastHit hit;
    public Camera camera;
    private NavMeshAgent agent;
    public Transform arrowSpawnPoint; // Assign the transform from where the arrow is spawned
    public GameObject selectedEnemy;
    public Vector3 position;

    private bool isCooldown = false; // Cooldown flag
    private float cooldownTime = 10f; // Cooldown duration

    public float launchForce = 20f; // Initial force applied to the arrow
    public float arrowLifetime = 5f;
    public LayerMask enemyLayer; // Assign the enemy layer in the Inspector
    public float detectionRadius = 2f; // Radius to detect enemies around the arrow

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
                selectedEnemy = hit.transform.gameObject;
                position = hit.point;

                // Calculate direction ignoring the Y-axis for a horizontal rotation
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y; // Keep the player level on the X-Z plane

                // Make the player look at the target position
                agent.transform.LookAt(targetPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.S) && !isCooldown)
        {
            animator.SetTrigger("ArrowShower");
            ShootShower(position);
            StartCoroutine(Cooldown());
        }
    }

    void ShootShower(Vector3 rayhit)
    {
        rayhit.y += 10f;

        // Instantiate the arrow
        GameObject arrow = Instantiate(arrowPrefab, rayhit, Quaternion.identity);

        // Check for collisions using OverlapSphere
        StartCoroutine(CheckCollision(arrow));

        // Destroy the arrow after its lifetime
        Destroy(arrow, arrowLifetime);
    }

    IEnumerator CheckCollision(GameObject arrow)
    {
        float elapsedTime = 0f;

        while (elapsedTime < arrowLifetime)
        {
            // Use Physics.OverlapSphere to check for enemies
            Collider[] hitColliders = Physics.OverlapSphere(arrow.transform.position, detectionRadius, enemyLayer);

            foreach (Collider collider in hitColliders)
            {
                Debug.Log($"Arrow hit an enemy: {collider.gameObject.name}");

                // Perform any action on collision, e.g., destroy the enemy
                // Destroy(collider.gameObject);

                // Destroy the arrow
                Destroy(arrow);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
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
