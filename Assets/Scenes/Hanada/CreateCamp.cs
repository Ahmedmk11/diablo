using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class CreateCamp : MonoBehaviour
{
    public Transform player;
    public GameObject demonPrefab;
    public GameObject minionPrefab;
    public AnimatorController demonController;
    public AnimatorController minionController;
    public List<Vector3> campPositions;
    private float demonsPatrolRadius = 8f;
    private float minionsIdleRadius = 1.5f;
    private int numberOfMinions = 16;
    public CampManager campManagerPrefab;
    private CampManager campManagerInstance;
    private bool firstTime = true;
    private yarab yarabScript;
    public GameObject particleSystem;

    public GameObject runePrefab;
    public GameObject mainCamera;

    public Camera minimapCamera;

    private ArrayList demons;
    private ArrayList minions;


    private void Start()
    {
        yarabScript = GetComponent<yarab>();
        demons = new ArrayList();
        minions = new ArrayList();
    }

    void LateUpdate()
    {
        if (firstTime)
        {
            foreach (Vector3 campPosition in campPositions)
            {
                CampManager campManagerInstance = Instantiate(campManagerPrefab, campPosition, Quaternion.identity);
                campManagerInstance.transform.position = campPosition;
                campManagerInstance.player = player;
                campManagerInstance.campRadius = demonsPatrolRadius;
                campManagerInstance.centerPoint = campPosition;
                campManagerInstance.playerHealth = yarabScript.health;
                campManagerInstance.name = "Camp" + campPositions.IndexOf(campPosition);
                campManagerInstance.runePrefab = runePrefab;
                campManagerInstance.mainCamera = mainCamera;
                campManagerInstance.minimapCamera = minimapCamera;



    SpawnMinionsAndDemons(campManagerInstance, campPosition);
            }

            firstTime = false;
        }
    }

    void SpawnMinionsAndDemons(CampManager campManagerInstance, Vector3 centerPoint)
    {
        SpawnMinions(campManagerInstance, centerPoint);
        SpawnDemons(campManagerInstance, centerPoint);
    }

    void SpawnMinions(CampManager campManagerInstance, Vector3 centerPoint)
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(numberOfMinions)); 
        float spacing = minionsIdleRadius;

        int currentMinion = 0;
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (currentMinion >= numberOfMinions) 
                    break;

                float x = centerPoint.x + minionsIdleRadius / 2 + (col - gridSize / 2) * spacing;
                float z = centerPoint.z + minionsIdleRadius / 2 + (row - gridSize / 2) * spacing;

                Vector3 agentPosition = new Vector3(x, centerPoint.y, z);
                GameObject agent = Instantiate(minionPrefab, agentPosition, Quaternion.identity);
                
                agent.AddComponent<BoxCollider>();
                agent.GetComponent<BoxCollider>().center = new Vector3(0f, 0.9f, 0f);
                agent.GetComponent<BoxCollider>().size = new Vector3(1f, 1.7f, 1f);
                NavMeshAgent navMeshAgent = agent.AddComponent<NavMeshAgent>();
                navMeshAgent.speed = 0.5f;
                navMeshAgent.angularSpeed = 10f;
                navMeshAgent.stoppingDistance = 1.5f;
                
                Minion minionScript = agent.AddComponent<Minion>();
                minionScript.campManager = campManagerInstance;
                minionScript.player = player.gameObject;
                minionScript.yarabScript = yarabScript;

                Animator animator = agent.GetComponent<Animator>();
                animator.runtimeAnimatorController = minionController;
                animator.applyRootMotion = false;


                agent.tag = "Enemy";

                currentMinion++;
                minions.Add(agent);
            }
        }
    }

    void SpawnDemons(CampManager campManagerInstance, Vector3 centerPoint)
    {
        float angleStep = 180f;

        for (int i = 0; i < 2; i++)
        {
            float angle = i * angleStep;

            float x = centerPoint.x + demonsPatrolRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = centerPoint.z + demonsPatrolRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 agentPosition = new Vector3(x, centerPoint.y, z);

            GameObject agent = Instantiate(demonPrefab, agentPosition, Quaternion.identity);
            agent.AddComponent<BoxCollider>();
            agent.GetComponent<BoxCollider>().center = new Vector3(0f, 1f, 0f);
            agent.GetComponent<BoxCollider>().size = new Vector3(1f, 2f, 1f);

            NavMeshAgent navMeshAgent = agent.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = 0.5f;
            navMeshAgent.angularSpeed = 10f;
            navMeshAgent.avoidancePriority = i * 5;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.stoppingDistance = 1.5f;

            Demon demonScript = agent.AddComponent<Demon>();
            demonScript.campManager = campManagerInstance;
            demonScript.player = player.gameObject;
            demonScript.yarabScript = yarabScript;
            demonScript.particleSystem = particleSystem;
            demonScript.demonName = "Demon" + i;

            Animator animator = agent.GetComponent<Animator>();
            animator.runtimeAnimatorController = demonController;
            animator.applyRootMotion = false;

            agent.tag = "Enemy";
            demons.Add(agent);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 campPosition in campPositions)
        {
            Gizmos.DrawWireSphere(campPosition, demonsPatrolRadius);
        }
    }

    public void changePlayerToFollow(Transform target)
    {
        if (demons != null)
        {
            foreach (GameObject demon in demons)
            {
                demon.GetComponent<Demon>().player = target.gameObject;
            }
        }
        if (minions != null)
        {
            foreach (GameObject minion in minions)
            {
                minion.GetComponent<Minion>().player = target.gameObject;
            }
        }
    }
}
