using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private Animator animator;
    public GameObject arrowPrefab; // Assign your arrow prefab
    private RaycastHit hit;
    public Camera camera;
    private NavMeshAgent agent;
    public Transform arrowSpawnPoint; // Assign the transform from where the arrow is spawned
    public GameObject selectedEnemy;
    public bool isShower = true;
    public bool isDash = true;

    private bool isCooldown = false; // Cooldown flag
    private float cooldownTime = 1f; // Cooldown duration


    public float launchForce = 20f; // Initial force applied to the arrow
    public float arrowLifetime = 5f;

    void Start()
    {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1) && isShower && isDash) // Right-click
        {
            // Create a ray from the camera to the mouse position
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);



            if (Physics.Raycast(ray, out RaycastHit hit)) // Check if the ray hit something
            {
                if (hit.transform.tag == "Enemy")
                {
                    selectedEnemy = hit.transform.gameObject;


                    // Calculate direction ignoring the Y-axis for a horizontal rotation
                    Vector3 targetPosition = hit.point;
                    targetPosition.y = transform.position.y; // Keep the player level on the X-Z plane

                    // Make the player look at the target position
                    agent.transform.LookAt(targetPosition);
                }
            }
        }
        // Check for the key press (K) to shoot an arrow
        if (!isCooldown)
        {
            if (selectedEnemy != null)
            {
                // Set the "Arrow" trigger in the Animator
                animator.SetTrigger("Arrow");
                FindObjectOfType<audiomanager>().PlaySFX("arrowHitSFX");
                findAndDamageEnemy(5, selectedEnemy.GetComponent<Collider>());
                StartCoroutine(DelayFunctionCall());
                StartCoroutine(Cooldown());
            }
            selectedEnemy = null;
        }

        // Rotate player towards the mouse position on left click
        
    }

    void ShootArrow()
    {
        Debug.Log("Arrow Shot");

        // Adjust position and rotation of the arrow
        Quaternion adjustedRotation = Quaternion.Euler(
            arrowPrefab.transform.rotation.eulerAngles.x,
            arrowSpawnPoint.rotation.eulerAngles.y,
            arrowSpawnPoint.rotation.eulerAngles.z
        );
        Vector3 adjustedPosition = new Vector3(
            arrowSpawnPoint.position.x - 0.2f,
            arrowSpawnPoint.position.y + 1.3f,
            arrowSpawnPoint.position.z + 0.1f
        );

        // Instantiate the arrow with the adjusted rotation
        GameObject arrow = Instantiate(arrowPrefab, adjustedPosition, adjustedRotation);

        // Add initial force to the arrow for its trajectory
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.AddForce(arrowSpawnPoint.forward * launchForce, ForceMode.Impulse);
        }

        // Destroy the arrow after a set lifetime to avoid clutter
        Destroy(arrow, arrowLifetime);
    }

    IEnumerator DelayFunctionCall()
    {
        // Wait for 0.8 seconds
        yield return new WaitForSeconds(0.8f);

        // Call the ShootArrow function
        ShootArrow();
    }
    IEnumerator Cooldown()
    {
        // Set cooldown flag
        isCooldown = true;

        StartCoroutine(CooldownRoutine(GameObject.Find("Basic").GetComponent<UnityEngine.UI.Image>(), (int)cooldownTime, "Basic"));


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
