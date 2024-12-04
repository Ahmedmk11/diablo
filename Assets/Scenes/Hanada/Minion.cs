using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    public CampManager campManager;
    public Animator animator;
    public AnimatorController minionController;
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
        }
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
            Debug.Log("Minion starts following the player");
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.stoppingDistance = 0f;
        }

        FaceMovementDirection();
        // remove
        if (hp < 20) print("Minion:"+hp);
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

    public void DropXP()
    {
        Debug.Log("Minion XP dropped");
    }

    public void MeleeAttack()
    {
        Debug.Log("Minion Punch");
    }

    public void takeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Minion died");
    }
}