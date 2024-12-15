using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            StopAgentAtCurrentPosition();   
            StartCoroutine(DelayedShootBomb());
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
        //GameObject arrow = Instantiate(arrowPrefab, adjustedPosition, adjustedRotation);
        GameObject smokeP = Instantiate(smoke, spawnPosition, spawnRotation);

        // Add initial force to the arrow for its trajectory
       

        // Detect enemies in range of the smoke
        Collider[] nearbyEnemies = Physics.OverlapSphere(smokeP.transform.position, detectionRadius);
        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                findAndStunEnemy(enemy);
                Debug.Log($"Enemy detected: {enemy.name}");
            }
        }

        // Destroy the arrow and smoke after a set lifetime
        //Destroy(arrow, arrowLifetime);
        Destroy(smokeP, 1f);
    }
    private void StopAgentAtCurrentPosition()
    {
        if (agent != null)
        {
            agent.isStopped = true; // Stop the agent
            agent.ResetPath(); // Clear the current path to prevent further movement
            Debug.Log("Agent stopped at its current position.");
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not initialized!");
        }

    }

    IEnumerator Cooldown()
    {
        // Set cooldown flag
        isCooldown = true;

        StartCoroutine(CooldownRoutine(GameObject.Find("Defensive").GetComponent<UnityEngine.UI.Image>(), (int) cooldownTime));

        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);

        // Reset cooldown flag
        isCooldown = false;
    }
    IEnumerator DelayedShootBomb()
    {
        yield return new WaitForSeconds(2f); // Delay for 2 seconds
        ShootBomb(); // Call the original ShootBomb method
    }

    private void findAndStunEnemy(Collider collider)
    {
        LilithBehavior lilith = collider.GetComponent<LilithBehavior>();
        lilithphase2testingscript lilith2 = collider.GetComponent<lilithphase2testingscript>();
        Minion minion = collider.GetComponent<Minion>();
        Demon demon = collider.GetComponent<Demon>();

        if (lilith != null)
        {
            lilith.takeStun();
        }
        else if (lilith2 != null)
        {
            lilith2.takeStun();
        }
        else if (minion != null)
        {
            minion.Stun();
        }
        else if (demon != null)
        {
            demon.Stun();
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
