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
    private bool coolWindw = true;
    
    
    public Vector3 position;

    private bool isCooldown = false; // Cooldown flag
    private float cooldownTime = 5f; // Cooldown duration
    private float dashDelay = 1.3f; // Delay before executing the dash

    private bool isSelecting = false;

    private bool isSpeedModified = false;
    public PlayerAnimationTrigger arrow;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        arrow = GetComponent<PlayerAnimationTrigger>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSelecting && coolWindw) // Right-click
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
                StartCoroutine(Sound());
                StartCoroutine(ExecuteDashWithDelay(position));
                StartCoroutine(AdjustCooldown());
                arrow.isDash = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isCooldown && coolWindw)
        {
            isSelecting = true;
            arrow.isDash = false;
            
        }

        if (isSpeedModified && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                agent.speed = agent.speed / 2; // Reset the speed
                isSpeedModified = false; // Mark that the speed has been reset
                Debug.Log("Agent has reached the destination, speed reset.");
            }
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
        if (!isSpeedModified)
        {
            StartCoroutine(SpeedBoost(position));
        }
    }
    private IEnumerator SpeedBoost(Vector3 position)
    {
        // Set the destination and boost the speed
        isSpeedModified = true;
        float originalSpeed = agent.speed;
        agent.speed = originalSpeed * 10; // Increase speed
        agent.SetDestination(position);

        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        // Reset the speed
        agent.speed = originalSpeed;
        isSpeedModified = false;
    }

    IEnumerator AdjustCooldown()
    {
        coolWindw = false;
        yield return new WaitForSeconds(3);
        coolWindw = true;
        StartCoroutine(Cooldown());
     }

    IEnumerator Sound()
    {
        yield return new WaitForSeconds(1);
        FindObjectOfType<audiomanager>().PlaySFX("dashSFX");


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
