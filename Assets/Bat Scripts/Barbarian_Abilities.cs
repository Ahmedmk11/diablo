using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

public class Barbarian_Abilities : MonoBehaviour
{
    private Animator animator;
    private Transform targetEnemy; // The currently selected enemy
    private bool isAttacking = false; // Flag to indicate attacking state

    private bool isUltimateActive = false; // Flag to indicate if the ultimate ability is active
    private Vector3 ultimateTargetPosition; // The position to reach for the ultimate ability
    private bool isUltimateMoving = false; // Flag to control when the player is moving to the target
    private bool canMoveAfterUltimate = true;  // Flag to check if normal movement can happen after ultimate ability
    private bool isSelectingUltimatePosition = false;  // Flag to track whether the player is selecting a position

    public float attackRange = 2f; // Range within which the Basic Ability is triggered
    public float moveSpeed = 5f; // Speed for moving towards the enemy
    public float rotationSpeed = 10f; // Speed for rotating towards the enemy

    public GameObject shieldLight; // The Point Light (Shield)
   
    private NavMeshAgent agent; // NavMeshAgent for movement

    // Cooldown durations for abilities
    private Dictionary<string, float> abilityCooldowns = new Dictionary<string, float>
    {
        { "Basic", 1f },
        { "Defensive", 10f },
        { "Wildcard", 5f },
        { "Ultimate", 10f }
    };

    // Cooldown timers
    private Dictionary<string, float> abilityCooldownTimers = new Dictionary<string, float>
    {
        { "Basic", 0f },
        { "Defensive", 0f },
        { "Wildcard", 0f },
        { "Ultimate", 0f }
    };

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>(); // Initialize NavMeshAgent

        // Ensure shield light and glow effect are initially inactive
        if (shieldLight != null) shieldLight.SetActive(false);

    }

    private void Update()
    {
        HandleInput();  // Handle other inputs such as abilities
        UpdateCooldownTimers();  // Update cooldown timers for all abilities

        // Handle the ultimate ability
        HandleUltimateAbility();

        // Handle normal movement, for example, using left-click to move the player
        if (targetEnemy != null && !isAttacking)
        {
            MoveAndFaceEnemy();  // Continue moving toward the enemy if needed
        }

        // If ultimate is active, disable normal movement
        if (isUltimateActive && !isUltimateMoving)
        {
            agent.isStopped = true;  // Prevent movement until position is selected
        }
        else if (canMoveAfterUltimate)
        {
            agent.isStopped = false;  // Allow normal movement after the ultimate is complete
        }
    }

    private void HandleInput()
    {
        // Right-click to select an enemy or a position for ultimate
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            if (isSelectingUltimatePosition) // If ultimate ability is activated, select the position
            {
                SelectUltimatePosition();
            }
            else
            {
                SelectEnemy(); // Select an enemy for the basic or wildcard abilities
            }
        }

        // Defensive Ability: Press W
        if (Input.GetKeyDown(KeyCode.W) && !IsAbilityOnCooldown("Defensive"))
        {
            TriggerDefensiveAbility();
        }

        // Wildcard Ability: Press Q
        if (Input.GetKeyDown(KeyCode.Q) && !IsAbilityOnCooldown("Wildcard"))
        {
            TriggerWildcardAbility();
        }

        // Ultimate Ability: Press E
        if (Input.GetKeyDown(KeyCode.E) && !IsAbilityOnCooldown("Ultimate") && !isUltimateActive)
        {
            TriggerUltimateAbility(); // Set up to select position
        }

        // Reset triggers when in Idle state
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ResetTriggers();
        }
    }

    private void SelectEnemy()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                targetEnemy = hit.collider.transform;
                Debug.Log($"Enemy selected: {targetEnemy.name}");

                // Start moving towards the enemy using NavMeshAgent
                agent.SetDestination(targetEnemy.position);
                agent.speed = moveSpeed; // Ensure the speed is set
            }
            else
            {
                Debug.Log("Hit something, but it's not an enemy");
            }
        }
        else
        {
            Debug.Log("No object hit");
        }
    }

    private void MoveAndFaceEnemy()
    {
        if (targetEnemy == null) return;

        // Calculate distance to the enemy
        float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);

        if (distanceToEnemy > attackRange)
        {
            // Continue moving towards the enemy
            agent.isStopped = false; // Ensure the agent is not stopped
            agent.SetDestination(targetEnemy.position); // Keep updating the destination
            RotateTowardsEnemy(); // Face the enemy while moving
        }
        else
        {
            // Stop the agent and trigger the attack
            agent.isStopped = true;
            agent.ResetPath(); // Clear the NavMeshAgent path to avoid residual movement
            StartCoroutine(AttackEnemy());
        }
    }





    private void RotateTowardsEnemy()
    {
        if (targetEnemy == null) return;

        Vector3 directionToEnemy = (targetEnemy.position - transform.position).normalized;
        directionToEnemy.y = 0; // Ignore vertical rotation

        // Instantly rotate towards the enemy
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }


    private IEnumerator AttackEnemy()
    {
        if (isAttacking || IsAbilityOnCooldown("Basic")) yield break;

        isAttacking = true;

        // Trigger the Basic Ability animation
        animator.SetTrigger("TriggerBasic");
        StartCooldown("Basic"); // Start cooldown for the ability

        yield return new WaitForSeconds(1f); // Wait for the cooldown duration

        // Clear the target and allow movement again
        targetEnemy = null;
        isAttacking = false;
        agent.isStopped = false; // Re-enable movement
    }
    private IEnumerator ResetAttackStateAfterCooldown(float cooldownDuration)
    {
        yield return new WaitForSeconds(cooldownDuration);
        isAttacking = false; // Allow new actions
        targetEnemy = null;  // Clear the target
        agent.isStopped = false; // Re-enable movement
    }

    private void TriggerBasicAbility()
    {
        if (!isAttacking && !IsAbilityOnCooldown("Basic"))
        {
            isAttacking = true; // Prevent re-triggering until finished
            StartCooldown("Basic"); // Start the cooldown
            animator.SetTrigger("TriggerBasic");
            Debug.Log("Basic ability triggered.");

            // Reset attack state after animation or cooldown duration
            StartCoroutine(ResetAttackStateAfterCooldown(abilityCooldowns["Basic"]));
        }
    }


    private void TriggerDefensiveAbility()
    {
        StartCooldown("Defensive");

        if (shieldLight != null) 
            shieldLight.SetActive(true);

        animator.SetTrigger("TriggerDefensive");
        Debug.Log("Defensive ability triggered.");

        StartCoroutine(DisableShieldAfterDuration(3f));
    }

    private void TriggerWildcardAbility()
    {
        StartCooldown("Wildcard");
        animator.SetTrigger("TriggerWildCard");
        Debug.Log("Wildcard ability triggered.");
    }
    private void SelectUltimatePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ultimateTargetPosition = hit.point; // Set the target position
            Debug.Log($"Ultimate target position set to: {ultimateTargetPosition}");

            // Set the destination for the NavMeshAgent to move toward the target position
            agent.SetDestination(ultimateTargetPosition);
            agent.speed = moveSpeed; // Ensure proper movement speed
            agent.isStopped = false; // Ensure movement starts

            isUltimateMoving = true;  // Mark the ability as in movement state
            isUltimateActive = false; // Deactivate the ultimate ability once movement starts
            isSelectingUltimatePosition = false; // End the position selection state
            animator.SetTrigger("MoveUltimate");  // Continue ultimate ability animation
        }
    }
    private void TriggerUltimateAbility()
    {
        if (IsAbilityOnCooldown("Ultimate")) return;  // If on cooldown, do nothing

        StartCooldown("Ultimate");

        // Debugging log
        Debug.Log("Ultimate ability activated. Please select a target position with right-click.");

        isUltimateActive = true;  // Activate the ultimate ability
        isSelectingUltimatePosition = true;  // Enable position selection

        animator.SetTrigger("TriggerUltimate");  // Trigger the ultimate animation

        // Do not trigger movement until position is selected
    }


    private void HandleUltimateAbility()
    {
        if (!isUltimateActive) return;  // If the ultimate ability is not active, do nothing

        // Wait for right-click to specify the target position
        if (Input.GetMouseButtonDown(1))  // Right mouse click to select position
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ultimateTargetPosition = hit.point;  // Set the target position
                Debug.Log($"Ultimate target position set to: {ultimateTargetPosition}");

                // Set the destination for the NavMeshAgent to move toward the target position
                agent.SetDestination(ultimateTargetPosition);
                agent.speed = moveSpeed;  // Ensure the player moves at the correct speed
                agent.isStopped = false;  // Allow movement

                isUltimateMoving = true;  // Mark the ultimate ability as in movement state
            }
        }

        // If the player is moving toward the target, continue the ultimate ability animation
        if (isUltimateMoving)
        {
            // Check if the player has reached the target position
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                // Once the player reaches the target, complete the ultimate ability
                Debug.Log("Ultimate ability completed.");

                // Reset the ultimate ability flags
                isUltimateActive = false;
                isUltimateMoving = false;
                canMoveAfterUltimate = true;  // Allow normal movement again

                agent.isStopped = true;  // Stop the NavMeshAgent

                animator.SetTrigger("StopUltimate");  // Optionally trigger stopping animation
            }
        }
    }


    private void ResetTriggers()
    {
        animator.ResetTrigger("TriggerBasic");
        animator.ResetTrigger("TriggerDefensive");
        animator.ResetTrigger("TriggerWildCard");
        animator.ResetTrigger("TriggerUltimate");
        Debug.Log("Triggers reset.");
    }

    private void StartCooldown(string abilityName)
    {
        if (abilityCooldownTimers.ContainsKey(abilityName))
        {
            abilityCooldownTimers[abilityName] = abilityCooldowns[abilityName];
        }
    }

    private bool IsAbilityOnCooldown(string abilityName)
    {
        return abilityCooldownTimers[abilityName] > 0;
    }

    private void UpdateCooldownTimers()
    {
        List<string> keys = new List<string>(abilityCooldownTimers.Keys);
        foreach (var key in keys)
        {
            if (abilityCooldownTimers[key] > 0)
            {
                abilityCooldownTimers[key] -= Time.deltaTime;
                if (abilityCooldownTimers[key] < 0)
                {
                    abilityCooldownTimers[key] = 0;
                }
            }
        }
    }

    private IEnumerator DisableShieldAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (shieldLight != null) shieldLight.SetActive(false);

        Debug.Log("Shield Deactivated");
    }
}
