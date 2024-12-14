using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CampManager : MonoBehaviour
{
    public Transform player;
    private int maxDemonsAlerted = 1;
    private int maxMinionsAlerted = 5;
    public Vector3 centerPoint;
    public float campRadius;
    private List<Minion> minions = new List<Minion>();
    private List<Demon> demons = new List<Demon>();
    private List<Minion> alertedMinions = new List<Minion>();
    private List<Demon> alertedDemons = new List<Demon>();
    private Dictionary<Demon, Vector3> demonPositions = new Dictionary<Demon, Vector3>();
    private Dictionary<Minion, Vector3> minionPositions = new Dictionary<Minion, Vector3>();
    private bool isPlayerInsideCampRadius = false;
    private System.Random random = new System.Random();
    private bool playerDied = false;
    public int playerHealth;
    public string name;

    public GameObject runePrefab;
    public GameObject mainCamera;

    public Camera minimapCamera;

    private void Start()
    {
        float initialDistanceToCenter = Vector3.Distance(player.position, centerPoint);
        isPlayerInsideCampRadius = initialDistanceToCenter <= campRadius;
    }

    private void Update()
    {
        float distanceToCenter = Vector3.Distance(player.position, centerPoint);

        if (demons.Count != 0)
        {
            if (demons[0].yarabScript.health > 0 && !isPlayerInsideCampRadius && distanceToCenter <= campRadius)
            {
                Debug.Log("Player is in camp radius");
                isPlayerInsideCampRadius = true;
                AlertNearbyEntities();
            }
            else if (isPlayerInsideCampRadius && distanceToCenter > campRadius)
            {
                Debug.Log("Player out of camp radius");
                isPlayerInsideCampRadius = false;
                ResetNearbyEntities();
            }
        }
        
        if(demons.Count != 0)
        {
            if (!playerDied && demons[0].yarabScript.health <= 0)
            // if (!playerDied && playerHealth <= 0)
            {
                playerDied = true;
                ResetNearbyEntities();
                Debug.Log("Player died (testing reset)");
            }
        }
        
    }

    public void RegisterMinion(Minion minion)
    {
        if (!minions.Contains(minion))
        {
            minions.Add(minion);
            minionPositions[minion] = minion.transform.position;
        }
    }

    public void RegisterDemon(Demon demon)
    {
        if (!demons.Contains(demon))
        {
            demons.Add(demon);
            demonPositions[demon] = demon.transform.position;
        }
    }

    public void UnregisterMinion(Minion minion)
    {
        if (minions.Contains(minion))
        {
            minions.Remove(minion);
            minionPositions.Remove(minion);

            if (alertedMinions.Contains(minion))
            {
                alertedMinions.Remove(minion);
            }

            float distanceToCenter = Vector3.Distance(player.position, centerPoint);

            if (alertedMinions.Count < maxMinionsAlerted && distanceToCenter <= campRadius)
            {
                var outermostMinions = minions
                    .OrderBy(minion => Vector3.Distance(minion.transform.position, player.transform.position))
                    .ToList();

                foreach (Minion newMinion in outermostMinions)
                {
                    if (!alertedMinions.Contains(newMinion))
                    {
                        alertedMinions.Add(newMinion);
                        newMinion.StartFollowingPlayer(player.gameObject);
                        break;
                    }
                }
            }
        }

        if (IsCampDead())
        {
            print("Camp is dead");
            GameObject rune = Instantiate(runePrefab, 
                new Vector3(centerPoint.x, 7.2f, centerPoint.z), Quaternion.identity);
            rune.AddComponent<CollectableRune>();
        }
    }

    public void UnregisterDemon(Demon demon)
    {
        if (demons.Contains(demon))
        {
            demons.Remove(demon);
            demonPositions.Remove(demon);
        }

        if (alertedDemons.Contains(demon))
        {
            alertedDemons.Remove(demon);
        }

        float distanceToCenter = Vector3.Distance(player.position, centerPoint);

        if (alertedDemons.Count < maxDemonsAlerted && distanceToCenter <= campRadius)
        {
            foreach (Demon newDemon in demons)
            {
                if (!alertedDemons.Contains(newDemon))
                {
                    alertedDemons.Add(newDemon);
                    newDemon.StartFollowingPlayer(player.gameObject);
                    break;
                }
            }
        }

        if (IsCampDead())
        {
            print("Camp is dead");
            GameObject rune = Instantiate(runePrefab,
                new Vector3(centerPoint.x, 7.2f, centerPoint.z), Quaternion.identity);
            rune.GetComponentInChildren<Canvas>().worldCamera = minimapCamera.GetComponent<Camera>();

            rune.AddComponent<CollectableRune>();
        }
    }

    private bool IsCampDead()
    {
        return demons.Count == 0 && minions.Count == 0;
        // return true;
    }

    private void AlertNearbyEntities()
    {
        demons = demons.OrderBy(x => random.Next()).ToList();
        minions = minions.OrderBy(x => random.Next()).ToList();

        foreach (Demon demon in demons)
        {
            if (alertedDemons.Count >= maxDemonsAlerted)
                break;

            if (!alertedDemons.Contains(demon))
            {
                alertedDemons.Add(demon);
                demon.agent.ResetPath();
                demon.StartFollowingPlayer(player.gameObject);
                demon.patrolling = false;
            }
        }
        
        var outermostMinions = minions
            .OrderBy(minion => Vector3.Distance(minion.transform.position, player.transform.position))
            .ToList();

        foreach (Minion minion in outermostMinions)
        {
            if (alertedMinions.Count >= maxMinionsAlerted)
                break;

            if (!alertedMinions.Contains(minion))
            {
                alertedMinions.Add(minion);
                minion.StartFollowingPlayer(player.gameObject);
            }
        }
    }

    private void ResetNearbyEntities()
    {
        foreach (Demon demon in alertedDemons)
        {
            demon.goingBack = true;
            demon.agent.ResetPath();
            Debug.Log("before: Resetting demon path to initial");
            demon.agent.SetDestination(demonPositions[demon]);
            Debug.Log("after: Resetting demon path to initial");
            demon.StopFollowingPlayer();
            demon.patrolling = true;
        }

        foreach (Minion minion in alertedMinions)
        {
            minion.agent.ResetPath();
            minion.agent.SetDestination(minionPositions[minion]);
            minion.StopFollowingPlayer();
        }

        Debug.Log("Resetting alerted entities");

        alertedDemons.Clear();
        alertedMinions.Clear();
    }
}

public class CollectableRune : MonoBehaviour
{
    private CampManager campInstance;
    public void OnTriggerEnter(Collider other)
    {
        // Only collect if active and player touches it
        if (other.CompareTag("Player"))
        {
            print("Player collected rune");

            campInstance.mainCamera.GetComponent<yarab>().IncreaseRunes();

            // Destroy the potion object
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ensure the potion has a trigger collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true;

        campInstance = FindObjectOfType<CampManager>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Make it kinematic if you don't want it to be affected by physics
        }

        // Add a rotation animation to the rune
        gameObject.AddComponent<RotationAnimation>();
    }
}

public class RotationAnimation : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Continuous rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Floating up and down
        float newY = startPosition.y + Mathf.Sin((Time.time * floatSpeed) + randomOffset) * floatHeight;
        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }
}