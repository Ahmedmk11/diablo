using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian_Abilities : MonoBehaviour
{
    public Camera camera;

    private Animator animator;
    private Transform targetEnemy; // The currently selected enemy
    private bool isAttacking = false; // Flag to indicate attacking state

    private bool isUltimateActive = false; // Flag to indicate if the ultimate ability is active
    private Vector3 ultimateTargetPosition; // The position to reach for the ultimate ability
    private bool isUltimateMoving = false; // Flag to control when the player is moving to the target
    private bool canMoveAfterUltimate = true;  // Flag to check if normal movement can happen after ultimate ability
    private bool isSelectingUltimatePosition = false;  // Flag to track whether the player is selecting a position

    public float attackRange = 2f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public GameObject shieldLight; 
    public bool shieldActive = false; 
   
    private NavMeshAgent agent; // NavMeshAgent for movement

    public bool DefensiveAbilityLockedBarb = true;
    public bool WildcardAbilityLockedBarb = true;
    public bool UltimateAbilityLockedBarb = true;

    // Cooldown durations for abilities
    private Dictionary<string, float> abilityCooldowns = new Dictionary<string, float>
    {
        { "Basic", 1f },
        { "Defensive", 10f },
        { "Wildcard", 5f },
        { "Ultimate", 10f }
    };

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
        agent = GetComponent<NavMeshAgent>();

        if (shieldLight != null)
        {
            shieldLight.SetActive(false);
            shieldActive = false;
        }
    }

    private void Update()
    {
        HandleInput();  // Handle other inputs such as abilities
        UpdateCooldownTimers();  // Update cooldown timers for all abilities

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
        if(camera.GetComponent<yarab>().resetCooldowns)
        {
            abilityCooldownTimers["Basic"] = 0;
            abilityCooldownTimers["Defensive"] = 0;
            abilityCooldownTimers["Wildcard"] = 0;
            abilityCooldownTimers["Ultimate"] = 0;
            // camera.GetComponent<yarab>().resetCooldowns = false;
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
        if (Input.GetKeyDown(KeyCode.W) && !IsAbilityOnCooldown("Defensive") && !DefensiveAbilityLockedBarb)
        {
            TriggerDefensiveAbility();
        }

        // Wildcard Ability: Press Q
        if (Input.GetKeyDown(KeyCode.Q) && !IsAbilityOnCooldown("Wildcard") && !WildcardAbilityLockedBarb)
        {
            TriggerWildcardAbility();
        }

        // Ultimate Ability: Press E
        if (Input.GetKeyDown(KeyCode.E) && !IsAbilityOnCooldown("Ultimate") && !isUltimateActive && !UltimateAbilityLockedBarb)
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
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                targetEnemy = hit.collider.transform;
                Debug.Log($"Enemy selected: {targetEnemy.name}");

                // Start moving towards the enemy using NavMeshAgent
                print(targetEnemy.position);
                agent.SetDestination(targetEnemy.position);
                if (agent.hasPath)
                {
                    Debug.Log("Agent has path, moving.");
                }
                else if (agent.isOnNavMesh)
                {
                    Debug.Log("Agent is on the NavMesh but has no valid path.");
                }
                else
                {
                    Debug.Log("Agent is not on the NavMesh.");
                }
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

        // wahwah
        findAndDamageEnemy(5, null);

        yield return new WaitForSeconds(1f); // Wait for the ability duration

        // Start the cooldown only after the ability is done
        StartCooldown("Basic");

        // Clear the target and allow movement again
        targetEnemy = null;
        isAttacking = false;
        agent.isStopped = false; // Re-enable movement
    }


    private void TriggerDefensiveAbility(){
    if (shieldLight != null)
    {
        shieldLight.SetActive(true);
        shieldActive = true;
    }

    animator.SetTrigger("TriggerDefensive");
    Debug.Log("Defensive ability triggered.");

    StartCoroutine(DisableShieldAfterDuration(3f));
}

    private void TriggerWildcardAbility()
    {
        animator.SetTrigger("TriggerWildCard");
        Debug.Log("Wildcard ability triggered.");
        DamageWildCard();
        StartCooldown("Wildcard");
    }

    private void DamageWildCard()
    {
        float damageRadius = 2f;

        // Find all colliders within the damage radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (var collider in colliders)
        {
            // Check if the collider belongs to an enemy
            if (collider.CompareTag("Enemy"))
            {
                // wahwah
                findAndDamageEnemy(10, collider);
                Debug.Log($"Damaged enemy: {collider.name}");
            }
        }
    }
    private void SelectUltimatePosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
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
            animator.SetTrigger("TriggerUltimate");  // Continue ultimate ability animation
            DamageUltimate();

            // Start the cooldown only after the ultimate logic is complete
            StartCooldown("Ultimate");
        }
    }


    private void DamageUltimate()
    {
        // Define the width of the ultimate ability's area of effect
        float width = 1.5f;

        // Create a ray from the player to the ultimate destination
        Vector3 direction = (ultimateTargetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, ultimateTargetPosition);

        // Cast a SphereCast along the path to detect all enemies in the way
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, width, direction, distance);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // wahwah
                findAndDamageEnemy(100, hit.collider);
                Debug.Log($"Damaged enemy: {hit.collider.name}");
            }
        }
    }

    private void TriggerUltimateAbility()
    {
        if (IsAbilityOnCooldown("Ultimate")) return;  // If on cooldown, do nothing
        Debug.Log("Ultimate ability activated. Please select a target position with right-click.");
        isUltimateActive = true;  // Activate the ultimate ability
        isSelectingUltimatePosition = true;  // Enable position selection
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
        switch(abilityName)
        {
            case "Basic":
                StartCoroutine(startStartingTheCooldown("Basic", 0, abilityName));
                break;
            case "Defensive":
                StartCoroutine(startStartingTheCooldown("Defensive", 0, abilityName));
                break;
            case "Wildcard":
                StartCoroutine(startStartingTheCooldown("Wildcard", 2, abilityName));
                break;
            case "Ultimate":
                StartCoroutine(startStartingTheCooldown("Ultimate", 5, abilityName));
                break;
        }
        
    }

    private IEnumerator startStartingTheCooldown(string type, int time, string abilityName)
    {
        yield return new WaitForSeconds(time);
        if (type == "Basic") StartCoroutine(CooldownRoutine(GameObject.Find(type).GetComponent<UnityEngine.UI.Image>(), (int)abilityCooldowns[type], type));
        else
        StartCoroutine(CooldownRoutine(GameObject.Find(type).GetComponent<UnityEngine.UI.Image>(), (int)abilityCooldowns[type]));
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

        if (shieldLight != null)
        {
            shieldLight.SetActive(false);
            shieldActive = false;
        }

        Debug.Log("Shield Deactivated");

        // Start the cooldown only after the ability finishes
        StartCooldown("Defensive");
    }

    private void findAndDamageEnemy(int damage, Collider collider)
    {
        Transform target = collider == null ? targetEnemy : collider.transform;

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
