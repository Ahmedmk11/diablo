using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SorcererManager : MonoBehaviour
{
    public Camera camera;
    public GameObject fireball;
    private Animator animator;

    private bool isTeleporting = false;
    private bool isCastingClone = false;
    private bool isCastingInferno = false;


    public GameObject clonePrefab;
    public GameObject cloneExplosionPrefab;
    public float cloneLifetime = 5f;
    public float cloneExplosionRadius = 2f;
    public int cloneExplosionDamage = 20;

    public GameObject infernoPrefab;
    public float infernoDuration = 5f;
    public int infernoInitialDamage = 10;
    public int infernoDamagePerSecond = 2;
    public float infernoRadius = 3f;

    public float fireballCooldown = 1f;
    public float teleportCooldown = 10f;
    public float cloneCooldown = 15f;
    public float infernoCooldown = 20f;

    private float lastFireballTime = -Mathf.Infinity;
    private float lastTeleportTime = -Mathf.Infinity;
    private float lastCloneTime = -Mathf.Infinity;
    private float lastInfernoTime = -Mathf.Infinity;

    public bool DefensiveAbilityLockedSorc = true;
    public bool WildcardAbilityLockedSorc = true;
    public bool UltimateAbilityLockedSorc = true;

    public bool cloneActive = false;
    public Transform clonePosition;
    public RuntimeAnimatorController cloneAnimator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) & !isCastingClone & !isCastingInferno & !isTeleporting)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (Time.time >= lastFireballTime + fireballCooldown)
                    {

                        Vector3 directionToEnemy = (hit.collider.transform.position - transform.position).normalized;
                        directionToEnemy.y = 0;
                        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

                        animator.SetTrigger("Cast fireball");
                        FindObjectOfType<audiomanager>().PlaySFX("fireballSFX");
                        castFireBall(hit.collider.gameObject);
                        lastFireballTime = Time.time;
                        StartCoroutine(CooldownRoutine(GameObject.Find("Basic").GetComponent<UnityEngine.UI.Image>(), (int)fireballCooldown, "Basic"));
                    }
                    else
                    {
                        Debug.Log("Fireball is on cooldown");
                    }
                }
                else
                {
                    Debug.Log("Target is not an enemy");
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.W) && !DefensiveAbilityLockedSorc)
        {
            if (Time.time >= lastTeleportTime + teleportCooldown)
            {
                isTeleporting = true;
                Debug.Log("Select a teleport position");
            }
            else
            {
                Debug.Log("Teleport is on cooldown");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !WildcardAbilityLockedSorc)
        {
            if (Time.time >= lastCloneTime + cloneCooldown)
            {
                isCastingClone = true;
                Debug.Log("Select a position to place the clone");
            }
            else
            {
                Debug.Log("Clone is on cooldown");
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !UltimateAbilityLockedSorc)
        {
            if (Time.time >= lastInfernoTime + infernoCooldown)
            {
                isCastingInferno = true;
                Debug.Log("Select a position to place the inferno");
            }
            else
            {
                Debug.Log("Inferno is on cooldown");
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
                {
                    Vector3 directionToEnemy = (hit.collider.transform.position - transform.position).normalized;
                    directionToEnemy.y = 0;
                    Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

                    if (isCastingClone)
                    {
                        animator.SetTrigger("Cast Clone");
                        castClone(hit.point);
                        isCastingClone = false;
                        lastCloneTime = Time.time;
                        StartCoroutine(CooldownRoutine(GameObject.Find("Wildcard").GetComponent<UnityEngine.UI.Image>(), (int)cloneCooldown));

                    }
                    else if (isCastingInferno)
                    {
                        animator.SetTrigger("Cast Inferno");
                        FindObjectOfType<audiomanager>().PlaySFX("abilitySFX");
                        castInferno(hit.point);
                        isCastingInferno = false;
                        lastInfernoTime = Time.time;
                        StartCoroutine(CooldownRoutine(GameObject.Find("Ultimate").GetComponent<UnityEngine.UI.Image>(), (int)infernoCooldown));
                    }
                }
                else
                {
                    Debug.Log("Targeted position is not valid.");
                }
            }
        }

        if (isTeleporting && Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Teleport");
            castTeleport();
            lastTeleportTime = Time.time;
            StartCoroutine(CooldownRoutine(GameObject.Find("Defensive").GetComponent<UnityEngine.UI.Image>(), (int)teleportCooldown));
        }

        if (camera.GetComponent<yarab>().resetCooldowns)
        {
            lastCloneTime = -Mathf.Infinity;
            lastInfernoTime = -Mathf.Infinity;
            lastTeleportTime = -Mathf.Infinity;
            lastFireballTime = -Mathf.Infinity;
        }
    }


    void castFireBall(GameObject targetEnemy)
    {
        if (targetEnemy != null)
        {
            GameObject fireballClone = Instantiate(fireball, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);

            Vector3 directionToEnemy = (targetEnemy.transform.position - transform.position).normalized;

            fireballClone.GetComponent<Rigidbody>().AddForce(directionToEnemy * 600);

            Debug.Log($"Fireball casted at {targetEnemy.name}");
        }
        else
        {
            Debug.Log("No enemy target for fireball");
        }
    }


    void castTeleport()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 directionToEnemy = (hit.collider.transform.position - transform.position).normalized;
            directionToEnemy.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

            StartCoroutine(TeleportWithDelay(hit.point, 2.05f));
            Debug.Log("Teleported to " + hit.point);

            isTeleporting = false;
        }
    }
    IEnumerator TeleportWithDelay(Vector3 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = targetPosition;
        GetComponent<NavMeshAgent>().Warp(targetPosition);
        Debug.Log("Teleported to " + targetPosition);
    }
    void castInferno(Vector3 position)
    {

        position.y = position.y + 1.0f;
        GameObject inferno = Instantiate(infernoPrefab, position, Quaternion.identity);

        StartCoroutine(HandleInfernoDamage(inferno, position));
        Debug.Log("Inferno placed at " + position);
    }
    IEnumerator HandleInfernoDamage(GameObject inferno, Vector3 position)
    {
        float elapsedTime = 0f;

        DealDamageToEnemies(position, infernoRadius, infernoInitialDamage);

        while (elapsedTime < infernoDuration)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;

            DealDamageToEnemies(position, infernoRadius, infernoDamagePerSecond);
        }

        Destroy(inferno);
        Debug.Log("Inferno ended");
    }
    void DealDamageToEnemies(Vector3 position, float radius, int damage)
    {
        Collider[] hitEnemies = Physics.OverlapSphere(position, radius);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(enemy.transform.position, position);

                if (distance <= radius)
                {
                    findAndDamageEnemy(damage, enemy.transform);
                    Debug.Log($"Enemy {enemy.name} took {damage} damage");
                }
            }
        }
    }

    void castClone(Vector3 position)
    {
        FindObjectOfType<audiomanager>().PlaySFX("abilitySFX");
        GameObject clone = Instantiate(clonePrefab, position, Quaternion.identity);
        clone.GetComponent<Animator>().runtimeAnimatorController = cloneAnimator;
        cloneActive = true;
        clonePosition = clone.transform;

        StartCoroutine(CloneLifetime(clone));
        Debug.Log("Clone placed at " + position);
    }
    IEnumerator CloneLifetime(GameObject clone)
    {
        yield return new WaitForSeconds(cloneLifetime);


        Vector3 explosionPosition = clone.transform.position + new Vector3(0, 1.0f, 0);


        if (cloneExplosionPrefab != null)
        {
            Instantiate(cloneExplosionPrefab, explosionPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No explosion prefab assigned!");
        }

        Debug.Log("Clone exploded!");
        FindObjectOfType<audiomanager>().PlaySFX("explosionSFX");

        Collider[] hitEnemies = Physics.OverlapSphere(clone.transform.position, cloneExplosionRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(clone.transform.position, enemy.transform.position);
                if (distanceToEnemy <= cloneExplosionRadius)
                {
                    findAndDamageEnemy(cloneExplosionDamage, enemy.transform);
                    Debug.Log($"Enemy {enemy.name} took {cloneExplosionDamage} damage from clone explosion.");
                }
            }
        }

        Destroy(clone);
        cloneActive = false;
    }


    private void findAndDamageEnemy(int damage, Transform target)
    {
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
