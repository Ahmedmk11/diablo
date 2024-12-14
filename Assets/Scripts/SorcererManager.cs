using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class SorcererManager : MonoBehaviour
{
    public Camera camera;
    public GameObject fireball;
    // public GameObject Enemy;
    private Animator animator;

    private bool isTeleporting = false;
    private bool isCastingClone = false;
    private bool isCastingInferno = false;


    public GameObject clonePrefab; // Drag your clone prefab here
    public GameObject cloneExplosionPrefab; // Assign the particle prefab in the inspector
    public float cloneLifetime = 5f; // Duration before the clone explodes
    public float cloneExplosionRadius = 2f; // Radius of the explosion damage
    public int cloneExplosionDamage = 20; // Damage dealt by the explosion

    public GameObject infernoPrefab; // Assign your ring of fire prefab here
    public float infernoDuration = 5f; // Duration the ring of fire lasts
    public int infernoInitialDamage = 10; // Damage dealt immediately upon creation
    public int infernoDamagePerSecond = 2; // Damage dealt per second
    public float infernoRadius = 3f; // Radius of the inferno effect

    public float fireballCooldown = 1f;
    public float teleportCooldown = 10f;
    public float cloneCooldown = 10f;
    public float infernoCooldown = 15f;

    private float lastFireballTime = -Mathf.Infinity;
    private float lastTeleportTime = -Mathf.Infinity;
    private float lastCloneTime = -Mathf.Infinity;
    private float lastInfernoTime = -Mathf.Infinity;

    public bool DefensiveAbilityLockedSorc = true;
    public bool WildcardAbilityLockedSorc = true;
    public bool UltimateAbilityLockedSorc = true;

    public bool cloneActive = false;
    public Transform clonePosition;
    public AnimatorController cloneAnimator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) & !isCastingClone & !isCastingInferno & !isTeleporting) // Right mouse button is button 1
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy")) // Check if the clicked object is an enemy
                {
                    if (Time.time >= lastFireballTime + fireballCooldown)
                    {

                        Vector3 directionToEnemy = (hit.collider.transform.position - transform.position).normalized;
                        directionToEnemy.y = 0; 
                        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); 

                        animator.SetTrigger("Cast fireball");
                        FindObjectOfType<audiomanager>().PlaySFX("fireballSFX");
                        castFireBall(hit.collider.gameObject); // Pass the clicked enemy
                        lastFireballTime = Time.time; // Update last usage time
                        StartCoroutine(CooldownRoutine(GameObject.Find("Basic").GetComponent<UnityEngine.UI.Image>(), (int)fireballCooldown, "Basic"));
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


        if (Input.GetKeyDown(KeyCode.W) && !DefensiveAbilityLockedSorc)
        {
            if (Time.time >= lastTeleportTime + teleportCooldown)
            {
                isTeleporting = true; // Enable teleport mode
                Debug.Log("Select a teleport position");
            }
            else
            {
                Debug.Log("Teleport is on cooldown");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !WildcardAbilityLockedSorc)
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
        if (Input.GetKeyDown(KeyCode.E) && !UltimateAbilityLockedSorc)
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
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                Vector3 directionToEnemy = (hit.collider.transform.position - transform.position).normalized;
                directionToEnemy.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

                if (isCastingClone)
                {
                    animator.SetTrigger("Cast Clone");
                    castClone(hit.point); // Place clone at clicked position
                    isCastingClone = false; // Exit targeting mode
                    lastCloneTime = Time.time; // Update cooldown
                    StartCoroutine(CooldownRoutine(GameObject.Find("Wildcard").GetComponent<UnityEngine.UI.Image>(), (int)cloneCooldown));

                }
                else if (isCastingInferno)
                {
                    animator.SetTrigger("Cast Inferno");
                    FindObjectOfType<audiomanager>().PlaySFX("abilitySFX");
                    castInferno(hit.point); // Place inferno at clicked position
                    isCastingInferno = false; // Exit targeting mode
                    lastInfernoTime = Time.time; // Update cooldown
                    StartCoroutine(CooldownRoutine(GameObject.Find("Ultimate").GetComponent<UnityEngine.UI.Image>(), (int)infernoCooldown));
                }
            }
        }

        if (isTeleporting && Input.GetMouseButtonDown(1)) // Left-click to select position
        {
            animator.SetTrigger("Teleport");
            castTeleport();
            lastTeleportTime = Time.time; // Update last usage time
            StartCoroutine(CooldownRoutine(GameObject.Find("Defensive").GetComponent<UnityEngine.UI.Image>(), (int)teleportCooldown));
        }

        if (camera.GetComponent<yarab>().resetCooldowns)
        {
            lastCloneTime = -Mathf.Infinity;
            lastInfernoTime = -Mathf.Infinity;
            lastTeleportTime = -Mathf.Infinity;
            lastFireballTime = -Mathf.Infinity;
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
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
            Vector3 directionToEnemy = (hit.collider.transform.position - transform.position).normalized;
            directionToEnemy.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);
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
        GetComponent<NavMeshAgent>().Warp(targetPosition);
        Debug.Log("Teleported to " + targetPosition);
    }
    void castInferno(Vector3 position)
    {

        position.y = position.y+1.0f;
        // Instantiate the inferno at the selected position
        GameObject inferno = Instantiate(infernoPrefab, position, Quaternion.identity);

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
                // Calculate the distance between the enemy and the inferno's center
                float distance = Vector3.Distance(enemy.transform.position, position);

                // Only apply damage if the enemy is within the radius
                if (distance <= radius)
                {
                    findAndDamageEnemy(damage, enemy.transform);
                    Debug.Log($"Enemy {enemy.name} took {damage} damage");
                }
            }
        }
    }

        void castClone(Vector3 position)
    {
        // Instantiate the clone at the selected position
        FindObjectOfType<audiomanager>().PlaySFX("abilitySFX");
        GameObject clone = Instantiate(clonePrefab, position, Quaternion.identity);
        clone.GetComponent<Animator>().runtimeAnimatorController = cloneAnimator;
        cloneActive = true;
        clonePosition = clone.transform;

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

        // Trigger the explosion effect by instantiating the particle prefab

        Vector3 explosionPosition = clone.transform.position + new Vector3(0, 1.0f, 0); // Adjust the y offset as needed


        if (cloneExplosionPrefab != null) // Ensure the prefab is assigned
        {
            Instantiate(cloneExplosionPrefab, explosionPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No explosion prefab assigned!");
        }

        // Log and deal damage to nearby enemies
        Debug.Log("Clone exploded!");
        FindObjectOfType<audiomanager>().PlaySFX("explosionSFX");

        Collider[] hitEnemies = Physics.OverlapSphere(clone.transform.position, cloneExplosionRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // Ensure only enemies are affected
            {
                float distanceToEnemy = Vector3.Distance(clone.transform.position, enemy.transform.position);
                if (distanceToEnemy <= cloneExplosionRadius) // Check if within explosion radius
                {
                    findAndDamageEnemy(cloneExplosionDamage, enemy.transform);
                    Debug.Log($"Enemy {enemy.name} took {cloneExplosionDamage} damage from clone explosion.");
                }
            }
        }

        // Destroy the clone
        Destroy(clone);
        cloneActive = false;
    }


    private void findAndDamageEnemy(int damage, Transform target)
    {
        LilithBehavior lilith = target.GetComponent<LilithBehavior>();
        lilithphase2testingscript lilith2 = target.GetComponent<lilithphase2testingscript>();
        Minion minion = target.GetComponent<Minion>();
        Demon demon = target.GetComponent<Demon>();

        if (lilith != null)
        {
            if (damage == 100) damage = 20;
            lilith.takeDamage(damage);
        }
        else if (lilith2 != null)
        {
            if (damage == 100) damage = 20;
            lilith2.takeDamage(damage);
        }
        else if (minion != null)
        {
            minion.takeDamage(damage);
        }
        else if (demon != null)
        {
            demon.takeDamage(damage);
        }
    }

    private IEnumerator CooldownRoutine(UnityEngine.UI.Image img, int cooldown, string type = "")
    {
        // find tmp text with a specific name
        GameObject gameObject;
        if (type == "Basic") gameObject = img.transform.GetChild(2).gameObject;
        else gameObject = img.transform.GetChild(3).gameObject;

        TMP_Text timer = img.transform.Find("cooldown numerical").GetComponent<TMP_Text>();
        gameObject.SetActive(true);

        float decreasing = cooldown;
        // Get the fill GameObject and set its fill amount to 0
        Transform fill = img.transform.Find("Fill");
        UnityEngine.UI.Image fillImage = fill.GetComponent<UnityEngine.UI.Image>();
        fillImage.fillAmount = 0;

        // Increment the fill amount over the cooldown period
        float elapsed = 0;
        while (elapsed < cooldown)
        {
            decreasing -= Time.deltaTime;
            elapsed += Time.deltaTime;
            timer.text = decreasing.ToString("F0");
            fillImage.fillAmount = elapsed / cooldown;
            if (camera.GetComponent<yarab>().resetCooldowns)
            {
                fillImage.fillAmount = 1;
                camera.GetComponent<yarab>().resetCooldowns = false;
                break;
            }
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
