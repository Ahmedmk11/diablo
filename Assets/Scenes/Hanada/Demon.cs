using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class Demon : MonoBehaviour
{

    private float hp = 40;
    private float xp = 30;
    private float damageSwing = 10;
    private float damageExplosive = 15;
    public CampManager campManager;
    public AnimatorController demonController;
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
    public float maxStoppingDistance = 4f;
    private float currentStoppingDistance;
    public bool goingBack = false;

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
            Debug.Log("half patrol on");
            patrolling = true;
            halfPatrol = true;
            goingBack = false;
        }

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (followingPlayer)
        {
            Debug.Log("Demon starts following the player");
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.stoppingDistance = 0f;
        }

        if (followingPlayer && agent.remainingDistance <= 1f)
        {
            // randomly do one attack 

            // System.Random random = System.Random(0, 2);
            Debug.Log("bedan");

            // MeleeAttack();
            ExplosiveAttack();
        }
        else 
        {
            animator.SetBool("isSwinging", false);
            animator.SetBool("isThrowing", false);
        }

        FaceMovementDirection();
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

    public void DropXP()
    {
        Debug.Log("Demon XP dropped");
    }

    public void MeleeAttack()
    {
        Debug.Log("Demon Swing");
        animator.SetBool("isSwinging", true);
    }

    public void ExplosiveAttack()
    {
        Debug.Log("Explosion");
        animator.SetBool("isThrowing", true);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        animator.SetBool("isTakingDamage", true);
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Demon died");
        animator.SetBool("isDying", true);

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
                float actualPatrolDistance;

                if (halfPatrol) {
                    halfPatrol = false;
                    actualPatrolDistance = patrolDistance / 2;
                } else {
                    actualPatrolDistance = patrolDistance;
                }

                if (agent.tag.Contains('0')) {
                    if (direction) {
                        z = transform.position.z + actualPatrolDistance;
                    } else {
                        z = transform.position.z - actualPatrolDistance;
                    }
                } else {
                    if (!direction) {
                        z = transform.position.z + actualPatrolDistance;
                    } else {
                        z = transform.position.z - actualPatrolDistance;
                    }
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
}
