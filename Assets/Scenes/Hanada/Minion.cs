using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    public CampManager campManager;
    public Animator animator;
    public NavMeshAgent agent;
    private float hp = 20;
    private float xp = 10;
    private float damage = 5;
    private float speed = 2.5f;
    public bool followingPlayer = false;
    public GameObject player;
    public float minStoppingDistance = 1.5f;
    public float maxStoppingDistance = 2.5f;
    private float currentStoppingDistance;
    public yarab yarabScript;
    private bool isAttacking = false;


    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.speed = speed;
        agent.avoidancePriority = Random.Range(0, 100);

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        campManager.RegisterMinion(this);
    }

    private void Update()
    {
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
    }

    void FaceMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 directionToFace = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
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

    public void Punch()
    {
        if (yarabScript.health <= 0) {
            return;
        }

        Debug.Log("Minion punch");
        animator.SetTrigger("isPunching");
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = 0f;

        yarabScript.takeDamage((int)damage, "Minion", clipLength);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        
        Punch();

        yield return new WaitForSeconds(1.5f); 
    }

    public void TakeDamage(float damage)
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
        Debug.Log("Minion died");
        animator.SetTrigger("isDying");
        yarabScript.gainXP((int)xp);

        campManager.UnregisterMinion(this);

        // Destroy(gameObject, 2f);
    }
}