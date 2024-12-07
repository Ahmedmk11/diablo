using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bomb : MonoBehaviour
{
    private Animator animator;
    public GameObject arrowPrefab; // Assign your arrow prefab
    private RaycastHit hit;
    public Camera camera;
    private NavMeshAgent agent;
    public Transform arrowSpawnPoint; // Assign the transform from where the arrow is spawned
    public Vector3 position;
    public GameObject smoke;

    private bool isCooldown = false; // Cooldown flag
    private float cooldownTime = 15f; // Cooldown duration
    public float launchForce = 7f; // Initial force applied to the arrow
    public float arrowLifetime = 9f;

    public float detectionRadius = 5f; // Radius to check for nearby enemies
    public LayerMask enemyLayer; // Assign the enemy layer in the Inspector

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isCooldown)
        {
            animator.SetTrigger("Bomb");
            ShootBomb();
            StartCoroutine(Cooldown());
        }
    }

    void ShootBomb()
    {
        Quaternion adjustedRotation = Quaternion.LookRotation(arrowSpawnPoint.forward);
        Vector3 adjustedPosition = new Vector3(
            arrowSpawnPoint.position.x,
            arrowSpawnPoint.position.y,
            arrowSpawnPoint.position.z
        );
        Vector3 spawnPosition = arrowSpawnPoint.position;
        Quaternion spawnRotation = Quaternion.LookRotation(arrowSpawnPoint.forward);

        // Instantiate the arrow
        GameObject arrow = Instantiate(arrowPrefab, adjustedPosition, adjustedRotation);
        GameObject smokeP = Instantiate(smoke, spawnPosition, spawnRotation);

        // Add initial force to the arrow for its trajectory
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.AddForce(arrowSpawnPoint.forward * launchForce, ForceMode.Impulse);
        }

        // Detect enemies in range of the smoke
        Collider[] nearbyEnemies = Physics.OverlapSphere(smokeP.transform.position, detectionRadius, enemyLayer);
        foreach (Collider enemy in nearbyEnemies)
        {
            // Stun the enemy
            findAndStunEnemy(enemy);
            Debug.Log($"Enemy detected: {enemy.name}");
        }

        // Destroy the arrow and smoke after a set lifetime
        Destroy(arrow, arrowLifetime);
        Destroy(smokeP, arrowLifetime);
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

    private void findAndStunEnemy(Collider collider)
    {
        LilithBehavior lilith = collider.GetComponent<LilithBehavior>();
        lilithphase2testingscript lilith2 = collider.GetComponent<lilithphase2testingscript>();
        Minion minion = collider.GetComponent<Minion>();
        Demon demon = collider.GetComponent<Demon>();

        if (lilith != null)
        {
            //lilith.takeStun();
        }
        else if (lilith2 != null)
        {
            //lilith2.takeStun();
        }
        else if (minion != null)
        {
            //minion.takeStun();
        }
        else if (demon != null)
        {
            //demon.takeStun();
        }
    }
}
