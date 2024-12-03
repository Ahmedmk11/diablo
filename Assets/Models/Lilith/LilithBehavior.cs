using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LilithBehavior : MonoBehaviour
{
    public Animator animator; // Reference to Animator
    public GameObject minionPrefab; // Minion prefab (Ch25_nonPBR)
    public float attackInterval = 5f; // Time between attacks
    public float rotationSpeed = 5f; // Speed at which Lilith rotates towards the cursor
    public float summonDistance = 1.5f; // Distance in front of Lilith to summon minions
    public int maxMinions = 3; // Maximum number of minions Lilith can summon

    private GameObject[] activeMinions; // Array to track currently summoned minions
    private bool isSummoning = true; // Alternate between summon and divebomb

    private void Start()
    {
        activeMinions = new GameObject[maxMinions];
        StartCoroutine(Phase1AttackLoop());
    }

    private void Update()
    {
        RotateTowardsCursor();
    }

    private IEnumerator Phase1AttackLoop()
    {
        while (true) // Infinite loop for testing
        {
            if (isSummoning)
            {
                PerformSummon();
            }
            else
            {
                PerformDivebomb();
            }

            // Alternate attacks
            isSummoning = !isSummoning;

            yield return new WaitForSeconds(attackInterval); // Wait before the next attack
        }
    }

    private void PerformSummon()
{
    Debug.Log("Lilith is summoning!");
    animator.SetTrigger("Summon");

    if (AreAllMinionsDefeated())
    {
        // Define offsets for minion positions relative to Lilith
        Vector3[] offsets = new Vector3[]
        {
            transform.forward * summonDistance,               // Front
            transform.forward + transform.right * 2f, // Right
            transform.forward - transform.right * 2f  // Left
        };

        for (int i = 0; i < maxMinions; i++)
        {
            // Calculate spawn position based on offsets
            Vector3 spawnPosition = transform.position + offsets[i];

            // Instantiate and disable minion
            GameObject newMinion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            newMinion.SetActive(false); // Initially disable

            activeMinions[i] = newMinion; // Store reference
        }

        // Enable minions after summon animation begins
        StartCoroutine(EnableMinionsAfterSummon());
    }
    else
    {
        Debug.Log("Previous minions are still alive. Cannot summon.");
    }
}


    private IEnumerator EnableMinionsAfterSummon()
    {
        // Wait for the summon animation to complete (adjust delay as needed)
        yield return new WaitForSeconds(1.0f);

        foreach (GameObject minion in activeMinions)
        {
            if (minion != null)
            {
                minion.SetActive(true);
            }
        }

        Debug.Log("Minions enabled after summoning!");
    }

    private void PerformDivebomb()
    {
        Debug.Log("Lilith is performing Divebomb!");
        animator.SetTrigger("Divebomb");
        // Add divebomb logic here if needed (e.g., damage player)
    }

    private void RotateTowardsCursor()
    {
        // Get the mouse position in the world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Get the point where the ray hits
            Vector3 targetPosition = hitInfo.point;

            // Calculate the direction to the target position
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Ignore the Y axis to keep Lilith upright

            // Smoothly rotate towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private bool AreAllMinionsDefeated()
    {
        foreach (GameObject minion in activeMinions)
        {
            if (minion != null) return false;
        }
        return true;
    }

    private void takeDamage()
    {
        // Placeholder for taking damage
    }
}
