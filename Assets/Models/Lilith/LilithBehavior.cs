using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilithBehavior : MonoBehaviour
{
    public RuntimeAnimatorController animatorController; // Reference to Animator Controller
    public GameObject minionPrefab; // Minion prefab (Ch25_nonPBR)
    public float attackInterval = 5f; // Time between attacks
    public int maxMinions = 3; // Maximum number of minions Lilith can summon

    public int health = 50;

    private Animator animator; // Internal reference to Animator component
    private GameObject[] activeMinions; // Array to track currently summoned minions

    private void Start()
    {
        // Ensure Animator component is attached and assign the controller
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on Lilith!");
            return;
        }

        if (animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }
        else
        {
            Debug.LogError("No Animator Controller assigned to Lilith!");
        }

        activeMinions = new GameObject[maxMinions];
        StartCoroutine(Phase1AttackLoop());
    }

    private IEnumerator Phase1AttackLoop()
    {
        while (true)
        {
            // Check if all minions are defeated
            if (AreAllMinionsDefeated())
            {
                PerformSummon();
            }
            else
            {
                PerformDivebomb();
            }

            yield return new WaitForSeconds(attackInterval);
        }
    }

    public void takeDamage(int damage)
    {
        // health -= damage;
        // Debug.Log($"Lilith took {damage} damage. Remaining health: {health}");

        // // Play hit reaction animation
        // animator.SetTrigger("HitReaction");

        if (AreAllMinionsDefeated())
        {
            // Play dying animation and disable behavior
            health -= damage;
            animator.SetTrigger("HitReaction");

        }
    }

    private void PerformSummon()
    {
        Debug.Log("Lilith is summoning!");
        animator.SetBool("Summon", true);

        // Spawn minions at random positions
        for (int i = 0; i < maxMinions; i++)
        {
            float randomX = Random.Range(-40f, 40f);
            float randomZ = Random.Range(-15f, 32f);
            Vector3 spawnPosition = new Vector3(randomX, transform.position.y, randomZ);

            GameObject newMinion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            newMinion.SetActive(false); // Initially disable
            activeMinions[i] = newMinion;
        }

        // Enable minions after the summoning animation
        StartCoroutine(EnableMinionsAfterSummon());
    }

    private IEnumerator EnableMinionsAfterSummon()
    {
        yield return new WaitForSeconds(1.0f); // Adjust to match animation duration

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
        animator.SetBool("Summon", false);
        // Add divebomb logic if needed
    }

    private bool AreAllMinionsDefeated()
    {
        foreach (GameObject minion in activeMinions)
        {
            if (minion != null) return false;
        }
        return true;
    }
}
