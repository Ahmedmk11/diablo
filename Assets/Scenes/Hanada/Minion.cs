using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    public CampManager campManager;
    public Animator animator;
    public NavMeshAgent agent;
    public float hp = 20;
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
    private bool isStunned = false;
    public float playerHp;

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

        if (campManager != null)
        {
            campManager.RegisterMinion(this);
            campManager.playerHp = playerHp;
        }

        transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<Slider>().value = 1;

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

        // if following player face player

        if (followingPlayer)
        {
            Vector3 directionToFace = player.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToFace);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
        }

        if (hp <= 0)
        {
            agent.ResetPath();
            StopFollowingPlayer();
        }
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

    public void Stun()
    {
        isStunned = true;
        agent.isStopped = true;
        animator.SetTrigger("isStunned");
        StartCoroutine(Unstun());
    }

    IEnumerator Unstun()
    {
        yield return new WaitForSeconds(5f);
        isStunned = false;
        agent.isStopped = false;
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

        animator.SetTrigger("isPunching");
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;

        yarabScript.takeDamage((int)damage, "Minion", clipLength);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        
        if (!isStunned)
        {
            Punch();
        }

        yield return new WaitForSeconds(5f); 
        isAttacking = false;
    }

    public void takeDamage(float damage)
    {
        hp -= damage;
        animator.SetTrigger("isTakingDamage");
        if (hp <= 0)
        {
            Die();
        }

        transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<Slider>().value = hp / 20f;

    }

    public void Die()
    {
        FindObjectOfType<audiomanager>().PlaySFX("enemyDeathSFX");
        animator.SetTrigger("isDying");
        yarabScript.gainXP((int)xp);

        if (yarabScript.level != 2)
        {
            campManager.UnregisterMinion(this);
        }
        agent.ResetPath();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;

        StartCoroutine(FlashThenHide(clipLength));
    }

    IEnumerator FlashThenHide(float clipLength)
    {
        yield return new WaitForSeconds(3 * clipLength);
        gameObject.SetActive(false);
    }
}
