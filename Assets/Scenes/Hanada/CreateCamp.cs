using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

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

/*    private void Start()
    {
        foreach (Vector3 campPosition in campPositions)
        {
            CampManager campManagerInstance = Instantiate(campManagerPrefab, campPosition, Quaternion.identity);
            campManagerInstance.transform.position = campPosition;
            campManagerInstance.player = player;
            campManagerInstance.campRadius = demonsPatrolRadius;
            campManagerInstance.centerPoint = campPosition;


            SpawnMinionsAndDemons(campManagerInstance, campPosition);
        }
    }*/

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
                NavMeshAgent navMeshAgent = agent.AddComponent<NavMeshAgent>();
                navMeshAgent.speed = 0.5f;
                navMeshAgent.angularSpeed = 10f;
                navMeshAgent.avoidancePriority = 10 + (currentMinion * 5);
                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                navMeshAgent.stoppingDistance = 1.5f;
                
                Minion minionScript = agent.AddComponent<Minion>();
                minionScript.minionController = minionController;
                minionScript.campManager = campManagerInstance;
                minionScript.player = player.gameObject;

                Debug.Log("ABOUZ");

                Animator animator = agent.GetComponent<Animator>();
                Debug.Log("AFTER ANIMATOR");
                animator.runtimeAnimatorController = minionController;
                animator.applyRootMotion = false;

                Debug.Log("WESELT");

                agent.tag = "Minion" + currentMinion;

                currentMinion++;
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

            NavMeshAgent navMeshAgent = agent.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = 0.5f;
            navMeshAgent.angularSpeed = 10f;
            navMeshAgent.avoidancePriority = i * 5;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.stoppingDistance = 1.5f;

            Demon demonScript = agent.AddComponent<Demon>();
            demonScript.demonController = demonController;
            demonScript.campManager = campManagerInstance;
            demonScript.player = player.gameObject;

            Animator animator = agent.GetComponent<Animator>();
            animator.runtimeAnimatorController = demonController;
            animator.applyRootMotion = false;

            agent.tag = "Demon" + i;
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
}
