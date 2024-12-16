using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Barbarian_Abilities : MonoBehaviour
{
    public Camera camera;

    private Animator animator;
    private Transform targetEnemy;
    private bool isAttacking = false; 

    private bool isUltimateActive = false; 
    private Vector3 ultimateTargetPosition; 
    private bool isUltimateMoving = false; 
    private bool canMoveAfterUltimate = true;  
    private bool isSelectingUltimatePosition = false;  

    public float attackRange = 2f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public GameObject shieldLight; 
    public bool shieldActive = false; 
   
    private NavMeshAgent agent; 

    public bool DefensiveAbilityLockedBarb = true;
    public bool WildcardAbilityLockedBarb = true;
    public bool UltimateAbilityLockedBarb = true;

    private bool DONTwild = false;
    private bool DONTult = false;
    private bool DONTdef = false;


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
        HandleInput();  
        UpdateCooldownTimers();  

        if (targetEnemy != null && !isAttacking)
        {
            MoveAndFaceEnemy();  
        }

        if (isUltimateActive && !isUltimateMoving)
        {
            agent.isStopped = true; 
        }
        else if (canMoveAfterUltimate)
        {
            agent.isStopped = false;  
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
        if (Input.GetMouseButtonDown(1)) 
        {
            if (isSelectingUltimatePosition) 
            {
                SelectUltimatePosition();
            }
            else
            {
                SelectEnemy(); 
            }
        }

        if (Input.GetKeyDown(KeyCode.W) && !IsAbilityOnCooldown("Defensive") && !DefensiveAbilityLockedBarb && !DONTdef)
        {
            TriggerDefensiveAbility();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !IsAbilityOnCooldown("Wildcard") && !WildcardAbilityLockedBarb && !DONTwild)
        {
            StopAgentAtCurrentPosition();
            TriggerWildcardAbility();
        }

        if (Input.GetKeyDown(KeyCode.E) && !IsAbilityOnCooldown("Ultimate") && !isUltimateActive && !UltimateAbilityLockedBarb && !DONTult)
        {
            TriggerUltimateAbility(); 
        }

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
                agent.speed = moveSpeed; 
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

        float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);

        if (distanceToEnemy > attackRange)
        {
            agent.isStopped = false; 
            agent.SetDestination(targetEnemy.position);
            RotateTowardsEnemy(); 
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath(); 
            StartCoroutine(AttackEnemy());
        }
    }

    private void RotateTowardsEnemy()
    {
        if (targetEnemy == null) return;

        Vector3 directionToEnemy = (targetEnemy.position - transform.position).normalized;
        directionToEnemy.y = 0; 

        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }


    private IEnumerator AttackEnemy()
    {
        if (isAttacking || IsAbilityOnCooldown("Basic")) yield break;

        isAttacking = true;

        animator.SetTrigger("TriggerBasic");

        // wahwah
        findAndDamageEnemy(5, null);

        yield return new WaitForSeconds(1f); 

        StartCooldown("Basic");

        targetEnemy = null;
        isAttacking = false;
        agent.isStopped = false; 
    }


    private void TriggerDefensiveAbility(){
        if (shieldLight != null)
        {
            FindObjectOfType<audiomanager>().PlaySFX("abilitySFX");
            shieldLight.SetActive(true);
            shieldActive = true;
        }

        animator.SetTrigger("TriggerDefensive");
        Debug.Log("Defensive ability triggered.");

        StartCooldown("Defensive");
        StartCoroutine(DisableShieldAfterDuration(3f));
}

    private void StopAgentAtCurrentPosition()
    {
        if (agent != null)
        {
            agent.isStopped = true; 
            agent.ResetPath(); 
            Debug.Log("Agent stopped at its current position.");
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not initialized!");
        }

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
        float damageRadius = 4f;

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                StartCoroutine(DamageEnemyWithDelay(collider, 10, 0.5f));
                // findAndDamageEnemy(10, collider);
                Debug.Log($"Damaged enemy: {collider.name}");
            }
        }
    }
    private void SelectUltimatePosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ultimateTargetPosition = hit.point; 
            Debug.Log($"Ultimate target position set to: {ultimateTargetPosition}");

            agent.SetDestination(ultimateTargetPosition);
            agent.speed = moveSpeed; 
            agent.isStopped = false; 

            isUltimateMoving = true;  
            isUltimateActive = false; 
            isSelectingUltimatePosition = false;
            animator.SetTrigger("TriggerUltimate");  
            FindObjectOfType<audiomanager>().PlaySFX("dashSFX");
            StartCoroutine(DamageUltimate());

            StartCooldown("Ultimate");
        }
    }


    /*private void DamageUltimate()
    {
        float width = 1.5f;

        Vector3 direction = (ultimateTargetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, ultimateTargetPosition);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, width, direction, distance);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                StartCoroutine(DamageEnemyWithDelay(hit.collider, 100, 2f));
            }
        }
    }*/

    private IEnumerator DamageUltimate()
    {
        float width = 1.5f; 
        float stepDistance = 1f; 
        float speed = 5f; 
        Vector3 startPosition = transform.position;
        Vector3 direction = (ultimateTargetPosition - startPosition).normalized;
        float totalDistance = Vector3.Distance(startPosition, ultimateTargetPosition);
        float traveledDistance = 0f;
        print(totalDistance);
        while (traveledDistance < totalDistance)
        {
            print(traveledDistance);
            Vector3 currentPosition = startPosition + direction * traveledDistance;

            RaycastHit[] hits = Physics.SphereCastAll(currentPosition, width, direction, stepDistance);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    StartCoroutine(DamageEnemyWithDelay(hit.collider, 100, 0f));
                }
            }

            // Move forward
            traveledDistance += speed * Time.deltaTime;

            transform.position = currentPosition;

            yield return null; 
        }

        transform.position = ultimateTargetPosition;
    }


    private IEnumerator DamageEnemyWithDelay(Collider enemyCollider, int damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        findAndDamageEnemy(damage, enemyCollider);

        Debug.Log($"Damaged enemy after delay: {enemyCollider.name}");
    }   
    private void TriggerUltimateAbility()
    {
        if (IsAbilityOnCooldown("Ultimate")) return; 
        Debug.Log("Ultimate ability activated. Please select a target position with right-click.");
        isUltimateActive = true; 
        isSelectingUltimatePosition = true;  
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
                StartCoroutine(startStartingTheCooldown("Defensive", 3, abilityName));
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
        if (type == "Wildcard")
        {
            DONTwild = true;
        }

        if (type == "Ultimate")
        {
            DONTult = true;
        }

        if (type == "Defensive")
        {
            DONTdef = true;
        }

        yield return new WaitForSeconds(time);

        if (type == "Wildcard")
        {
            DONTwild = false;
        }

        if (type == "Ultimate")
        {
            DONTult = false;
        }

        if (type == "Defensive")
        {
            DONTdef = false;
        }
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
        duration = 3f;
        yield return new WaitForSeconds(duration);

        if (shieldLight != null)
        {
            shieldLight.SetActive(false);
            shieldActive = false;
        }

        Debug.Log("Shield Deactivated");

        // StartCooldown("Defensive");
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
        GameObject gameObject;
        if (type == "Basic") gameObject = img.transform.GetChild(2).gameObject;
        else gameObject = img.transform.GetChild(3).gameObject;

        TMP_Text timer = img.transform.Find("cooldown numerical").GetComponent<TMP_Text>();
        gameObject.SetActive(true);

        float decreasing = cooldown;
        Transform fill = img.transform.Find("Fill");
        UnityEngine.UI.Image fillImage = fill.GetComponent<UnityEngine.UI.Image>();
        fillImage.fillAmount = 0;

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
