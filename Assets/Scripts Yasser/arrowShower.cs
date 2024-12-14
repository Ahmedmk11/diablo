using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject showerEffect;
    public Vector3 position;
    public PlayerAnimationTrigger arrow;

    private bool isCooldown = false; // Cooldown flag
    private float cooldownTime = 10f; // Cooldown duration

    public float launchForce = 20f; // Initial force applied to the arrow
    public float arrowLifetime = 5f;
    public LayerMask enemyLayer; // Assign the enemy layer in the Inspector
    public float detectionRadius = 6f; // Radius to detect enemies around the arrow

    private bool isSelecting = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        arrow = GetComponent<PlayerAnimationTrigger>();
    }

    void Update()
    {
        if (isSelecting && Input.GetMouseButtonDown(1)) // Right-click
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
                isSelecting = false;
                animator.SetTrigger("ArrowShower");
                ShootShower(position);
                StartCoroutine(Cooldown());
                arrow.isShower = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !isCooldown)
        {
            isSelecting = true;
            arrow.isShower = false;

        }
    }

    void ShootShower(Vector3 rayhit)
    {
        rayhit.y += 10f;
        Vector3 original = rayhit;
        original.y -= 9f;

        // Instantiate the arrow
        GameObject arrow = Instantiate(arrowPrefab, rayhit, Quaternion.identity);
       // GameObject effect = Instantiate(showerEffect, original, Quaternion.Euler(90f,0f,0f));

        // Check for collisions using OverlapSphere
        StartCoroutine(CheckCollision(arrow));

        // Destroy the arrow after its lifetime
        Destroy(arrow, arrowLifetime);
        //Destroy(effect, 1.5f);
        
    }

    IEnumerator CheckCollision(GameObject arrow)
    {
        float elapsedTime = 0f;

        while (elapsedTime < arrowLifetime)
        {
            // Use Physics.OverlapSphere to check for enemies
            Collider[] hitColliders = Physics.OverlapSphere(arrow.transform.position, detectionRadius);

            foreach (Collider collider in hitColliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    // Damage the enemy
                    findAndDamageEnemy(10, collider);
                    Debug.Log($"Arrow hit an enemy: {collider.gameObject.name}");

                    // Perform any action on collision, e.g., destroy the enemy
                    // Destroy(collider.gameObject);

                    // Destroy the arrow
                    
                }
               


            }
            //Destroy(arrow);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        // Set cooldown flag
        isCooldown = true;

        StartCoroutine(CooldownRoutine(GameObject.Find("Ultimate").GetComponent<UnityEngine.UI.Image>(), (int) cooldownTime));

        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);

        // Reset cooldown flag
        isCooldown = false;
    }

    private void findAndDamageEnemy(int damage, Collider collider)
    {
        LilithBehavior lilith = collider.GetComponent<LilithBehavior>();
        lilithphase2testingscript lilith2 = collider.GetComponent<lilithphase2testingscript>();
        Minion minion = collider.GetComponent<Minion>();
        Demon demon = collider.GetComponent<Demon>();

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
