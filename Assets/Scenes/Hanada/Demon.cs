using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Demon : MonoBehaviour
{
    private float hp = 40;
    private float xp = 30;
    private float damageSwing = 10;
    private float damageExplosive = 15;
    public CampManager campManager;
    public Animator animator;
    public NavMeshAgent agent;
    public bool patrolling = true;
    public float patrolSpeed = 2f;
    public float patrolDistance = 10f;
    public float waitTime = 4f;
    public bool direction = true;
    public bool halfPatrol = true;
    public bool followingPlayer = false;
    public GameObject player;
    public float minStoppingDistance = 1.5f;
    public float maxStoppingDistance = 2.5f;
    private float currentStoppingDistance;
    public bool goingBack = false;
    public int swings = 0;
    public yarab yarabScript;
    private bool isAttacking = false;
    public GameObject particleSystem;
    private ParticleSystem particleSystemInstance;

    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 1.5f;
        agent.avoidancePriority = Random.Range(0, 100);

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        campManager.RegisterDemon(this);

        StartCoroutine(PatrolRandomly());
    }

    private void Update()
    {
        if (goingBack)
        {
            patrolling = true;
            halfPatrol = true;
            goingBack = false;
        }

        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (followingPlayer)
        {
            agent.SetDestination(player.transform.position);

            if (!isAttacking && agent.velocity.sqrMagnitude <= 0.01f &&
                Vector3.Distance(transform.position, player.transform.position) <= agent.stoppingDistance)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            agent.stoppingDistance = 0f;
        }

        FaceMovementDirection();
        // remove
        if (hp < 40) print("Demon:" + hp);
    }

    void FaceMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 directionToFace = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * patrolSpeed);
        }
    }

    public void StartFollowingPlayer(GameObject player)
    {
        followingPlayer = true;
        this.player = player;
        currentStoppingDistance = Random.Range(minStoppingDistance, maxStoppingDistance);
        agent.stoppingDistance = currentStoppingDistance;
    }

    public void StopFollowingPlayer()
    {
        followingPlayer = false;
        player = null;
    }

    public void CreateExplosion(float clipLength)
    {
        particleSystemInstance = Instantiate(particleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particleSystemInstance.transform.position = transform.position + (transform.forward * 1.5f) + new Vector3(0, 1, 0);

        StartCoroutine(ActivateParticleSystemWithDelay(clipLength));
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        if (swings < 3)
        {
            // MeleeAttack();
            ExplosiveAttack();
        }
        else
        {
            ExplosiveAttack();
        }

        swings++;
        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
    }

    public void MeleeAttack()
    {
        if (yarabScript.health <= 0)
        {
            return;
        }

        Debug.Log("Demon Swing");
        animator.SetTrigger("isSwinging");

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;

        yarabScript.takeDamage((int)damageSwing, "Demon", clipLength);
    }

    public void ExplosiveAttack()
    {
        if (yarabScript.health <= 0)
        {
            return;
        }

        Debug.Log("Explosion");
        swings = 0;
        animator.SetTrigger("isThrowing");

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;
        CreateExplosion(clipLength);

        yarabScript.takeDamage((int)damageExplosive, "Demon", clipLength);
    }

    public void takeDamage(float damage)
    {
        hp -= damage;
        animator.SetTrigger("isTakingDamage");
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Demon died");
        animator.SetTrigger("isDying");
        yarabScript.gainXP((int)xp);

        campManager.UnregisterDemon(this);
        // Destroy(gameObject, 2f);
    }

    IEnumerator PatrolRandomly()
    {
        while (true)
        {
            if (patrolling)
            {
                yield return new WaitForSeconds(waitTime);

                float z;
                float actualPatrolDistance = halfPatrol ? patrolDistance / 2 : patrolDistance;
                halfPatrol = false;

                if (agent.tag.Contains('0'))
                {
                    z = direction ? transform.position.z + actualPatrolDistance : transform.position.z - actualPatrolDistance;
                }
                else
                {
                    z = !direction ? transform.position.z + actualPatrolDistance : transform.position.z - actualPatrolDistance;
                }

                direction = !direction;

                Vector3 nextPosition = new Vector3(transform.position.x, transform.position.y, z);

                agent.isStopped = false;
                agent.SetDestination(nextPosition);

                while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                {
                    yield return null;
                }

                agent.isStopped = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator ActivateParticleSystemWithDelay(float delay)
    {
        particleSystemInstance.Stop();
        yield return new WaitForSeconds(delay);
        particleSystemInstance.Play();
    }
}