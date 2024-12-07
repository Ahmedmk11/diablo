using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class LilithBehavior : MonoBehaviour
{
    public Camera cameraForYarab;
    public AnimatorController animatorController; // Reference to Animator Controller
    public GameObject minionPrefab; // Minion prefab (Ch25_nonPBR)
    public float attackInterval = 5f; // Time between attacks
    public int maxMinions = 3; // Maximum number of minions Lilith can summon

    public int health = 50;

    public bool firsttime = true;

    private Animator animator; // Reference to Animator component
    private GameObject[] activeMinions; // Array to track currently summoned minions

    public AnimatorController minionController;
    public GameObject player;


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
                if (firsttime)
                {
                    firsttime = false;
                    PerformSummon();
                }
                else{
                    yield return new WaitForSeconds(15.0f);
                    PerformSummon();
                }

            }
            else
            {
                yield return new WaitForSeconds(10.0f);
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
            float randomX = Random.Range(-57f, 10f);
            float randomZ = Random.Range(45f, 32f);
            Vector3 spawnPosition = new Vector3(randomX, 4.5f, randomZ);

            GameObject newMinion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);

            newMinion.AddComponent<BoxCollider>();
            NavMeshAgent navMeshAgent = newMinion.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = 0.5f;
            navMeshAgent.angularSpeed = 10f;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            Minion minionScript = newMinion.AddComponent<Minion>();
            minionScript.followingPlayer = true;
            minionScript.player = player.gameObject;


            Animator minionAnimator = newMinion.GetComponent<Animator>();
            minionAnimator.runtimeAnimatorController = minionController;
            minionAnimator.applyRootMotion = false;


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

        animator.SetTrigger("Dwarf Idle");
        Debug.Log("Lilith is performing Divebomb!");
        animator.SetTrigger("Divebomb");
        animator.SetBool("Summon", false);

        // Define the radius of effect and get the position for the sphere
        float radius = 5f; // Adjust radius as needed
        Vector3 explosionPosition = transform.position; // Assuming the divebomb's impact point is Lilith's position

        // Detect all colliders within the radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, radius);

        // Iterate through colliders and apply damage to enemies
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player")) // Ensure to replace with the correct tag
            {

              

                yarab yarab = cameraForYarab.GetComponent<yarab>();

                yarab.takeDamage(20); // Adjust damage value as needed
                Debug.Log($"Damaging {collider.name}");
            }
        }

        // Optional: Visualize the explosion area for debugging
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 0.1f, Color.red, 1.0f);
        Debug.DrawLine(transform.position + Vector3.right * radius, transform.position - Vector3.right * radius, Color.red, 1.0f);
        Debug.DrawLine(transform.position + Vector3.forward * radius, transform.position - Vector3.forward * radius, Color.red, 1.0f);
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
