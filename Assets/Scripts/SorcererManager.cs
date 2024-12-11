using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorcererManager : MonoBehaviour
{
    public GameObject fireball;
    public GameObject Enemy;
    private Animator animator;

    private bool isTeleporting = false;
    private bool isCastingClone = false;
    private bool isCastingInferno = false;


    public GameObject clonePrefab; // Drag your clone prefab here
    public float cloneLifetime = 5f; // Duration before the clone explodes
    public float cloneExplosionRadius = 5f; // Radius of the explosion damage
    public int cloneExplosionDamage = 20; // Damage dealt by the explosion

    public GameObject infernoPrefab; // Assign your ring of fire prefab here
    public float infernoDuration = 5f; // Duration the ring of fire lasts
    public int infernoInitialDamage = 10; // Damage dealt immediately upon creation
    public int infernoDamagePerSecond = 2; // Damage dealt per second
    public float infernoRadius = 5f; // Radius of the inferno effect

    public float fireballCooldown = 1f;
    public float teleportCooldown = 10f;
    public float cloneCooldown = 10f;
    public float infernoCooldown = 15f;

    private float lastFireballTime = -Mathf.Infinity;
    private float lastTeleportTime = -Mathf.Infinity;
    private float lastCloneTime = -Mathf.Infinity;
    private float lastInfernoTime = -Mathf.Infinity;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button is button 1
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy")) // Check if the clicked object is an enemy
                {
                    if (Time.time >= lastFireballTime + fireballCooldown)
                    {
                        animator.SetTrigger("Cast fireball");
                        castFireBall(hit.collider.gameObject); // Pass the clicked enemy
                        lastFireballTime = Time.time; // Update last usage time
                    }
                    else
                    {
                        Debug.Log("Fireball is on cooldown");
                    }
                }
                else
                {
                    Debug.Log("Target is not an enemy");
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time >= lastTeleportTime + teleportCooldown)
            {
                isTeleporting = true; // Enable teleport mode
                lastTeleportTime = Time.time; // Update last usage time
                Debug.Log("Select a teleport position");
            }
            else
            {
                Debug.Log("Teleport is on cooldown");
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time >= lastCloneTime + cloneCooldown)
            {
                isCastingClone = true; // Enable targeting mode for Clone
                Debug.Log("Select a position to place the clone");
            }
            else
            {
                Debug.Log("Clone is on cooldown");
            }
        }

        // Inferno targeting mode activation
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Time.time >= lastInfernoTime + infernoCooldown)
            {
                isCastingInferno = true; // Enable targeting mode for Inferno
                Debug.Log("Select a position to place the inferno");
            }
            else
            {
                Debug.Log("Inferno is on cooldown");
            }
        }

        // Handle left-click for Clone or Inferno placement
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (isCastingClone)
                {
                    animator.SetTrigger("Cast Clone");
                    castClone(hit.point); // Place clone at clicked position
                    isCastingClone = false; // Exit targeting mode
                    lastCloneTime = Time.time; // Update cooldown
                }
                else if (isCastingInferno)
                {
                    animator.SetTrigger("Cast Inferno");
                    castInferno(hit.point); // Place inferno at clicked position
                    isCastingInferno = false; // Exit targeting mode
                    lastInfernoTime = Time.time; // Update cooldown
                }
            }
        }

        if (isTeleporting && Input.GetMouseButtonDown(1)) // Left-click to select position
        {
            animator.SetTrigger("Teleport");
            castTeleport();
        }
    }


 void castFireBall(GameObject targetEnemy)
{
    if (targetEnemy != null)
    {
        // Instantiate the fireball at the player's position and rotation
        GameObject fireballClone = Instantiate(fireball, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);

        // Calculate the direction to the enemy
        Vector3 directionToEnemy = (targetEnemy.transform.position - transform.position).normalized;

        // Add force to the fireball to shoot it towards the enemy
        fireballClone.GetComponent<Rigidbody>().AddForce(directionToEnemy * 600);

        Debug.Log($"Fireball casted at {targetEnemy.name}");
    }
    else
    {
        Debug.Log("No enemy target for fireball");
    }
}


void castTeleport()
    {
            // Raycast to find the point clicked by the player
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
            // Check if the clicked position is walkable
            // Teleport the Sorcerer to the selected position
            StartCoroutine(TeleportWithDelay(hit.point, 2.05f)); // 0.5 seconds delay
            Debug.Log("Teleported to " + hit.point);

            // Exit teleport mode
            isTeleporting = false;
        }
    }
    IEnumerator TeleportWithDelay(Vector3 targetPosition, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Teleport the Sorcerer to the target position
        transform.position = targetPosition;
        Debug.Log("Teleported to " + targetPosition);
    }
    void castInferno(Vector3 position)
    {
        // Instantiate the inferno at the selected position
        GameObject inferno = Instantiate(infernoPrefab, new Vector3(position.x, position.y + 1, position.z), Quaternion.identity);

        // Start the damage over time coroutine
        StartCoroutine(HandleInfernoDamage(inferno, position));
        Debug.Log("Inferno placed at " + position);
    }
    IEnumerator HandleInfernoDamage(GameObject inferno, Vector3 position)
    {
        float elapsedTime = 0f;

        // Apply initial damage
        DealDamageToEnemies(position, infernoRadius, infernoInitialDamage);

        while (elapsedTime < infernoDuration)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second
            elapsedTime += 1f;

            // Apply damage per second
            DealDamageToEnemies(position, infernoRadius, infernoDamagePerSecond);
        }

        // Destroy the inferno after its duration
        Destroy(inferno);
        Debug.Log("Inferno ended");
    }

    void DealDamageToEnemies(Vector3 position, float radius, int damage)
    {
        // Find all enemies within the radius
        Collider[] hitEnemies = Physics.OverlapSphere(position, radius);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // Ensure only enemies are affected
            {
                // Apply damage to the enemy (assuming enemies have a script with a TakeDamage method)
                // enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage);
                Debug.Log($"Enemy {enemy.name} took {damage} damage");
            }
        }
    }
    void castClone(Vector3 position)
    {
        // Instantiate the clone at the selected position
        GameObject clone = Instantiate(clonePrefab, position, Quaternion.identity);

        //Animator playerAnimator = GetComponent<Animator>(); // Get the player's animator
        //if (playerAnimator != null)
        //{
        //    Animator cloneAnimator = clone.AddComponent<Animator>(); // Add Animator component to the clone
        //    cloneAnimator.runtimeAnimatorController = playerAnimator.runtimeAnimatorController; // Copy the runtime animator controller
        //}

        // Start the explosion timer for the clone
        StartCoroutine(CloneLifetime(clone));
        Debug.Log("Clone placed at " + position);
    }
    IEnumerator CloneLifetime(GameObject clone)
    {
        yield return new WaitForSeconds(cloneLifetime);

        // Trigger the explosion effect (you can add a particle effect here)
        Debug.Log("Clone exploded!");

        // Deal damage to nearby enemies
        Collider[] hitEnemies = Physics.OverlapSphere(clone.transform.position, cloneExplosionRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // Ensure only enemies are affected
            {
                // Apply damage to the enemy (assuming enemies have a script with a TakeDamage method)
               // enemy.GetComponent<EnemyHealth>()?.TakeDamage(cloneExplosionDamage);
            }
        }

        // Destroy the clone
        Destroy(clone);
    }
}
