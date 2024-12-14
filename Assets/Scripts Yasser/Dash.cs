using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private float cooldownTime = 5f; // Cooldown duration
    private float dashDelay = 2.3f; // Delay before executing the dash

    private bool isSelecting = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSelecting) // Right-click
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

                isSelecting = false;
                animator.SetTrigger("Dash");
                StartCoroutine(ExecuteDashWithDelay(position));
                StartCoroutine(Cooldown());
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isCooldown)
        {
            isSelecting = true;
            
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
        agent.SetDestination(position);
    }

    IEnumerator Cooldown()
    {
        // Set cooldown flag
        isCooldown = true;

        StartCoroutine(CooldownRoutine(GameObject.Find("Wildcard").GetComponent<UnityEngine.UI.Image>(), (int)cooldownTime));


        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);

        // Reset cooldown flag
        isCooldown = false;
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
