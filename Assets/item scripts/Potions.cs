using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Potions : MonoBehaviour
{
    public GameObject potionPrefab;
    public int numberOfPotions = 10;
    public float minDistanceBetweenPotions = 10f;
    public float searchRadius = 100f;

    public GameObject mainCamera;


    void Start()
    {
        SpawnPotions();
    }

    void SpawnPotions()
    {
        List<Vector3> spawnedPositions = new List<Vector3>();

        int attempts = 0;
        while (spawnedPositions.Count < numberOfPotions && attempts < 200)
        {
            Vector3 potentialPosition = GetRandomNavMeshPoint();

            if (potentialPosition != Vector3.zero && IsValidSpawnPoint(potentialPosition, spawnedPositions))
            {
                GameObject spawnedPotion = Instantiate(potionPrefab, potentialPosition, Quaternion.identity);

                // Add Collectable component
                CollectablePotion collectablePotion = spawnedPotion.AddComponent<CollectablePotion>();

                // Add animation component
                spawnedPotion.AddComponent<PotionAnimation>();

                spawnedPositions.Add(potentialPosition);
            }

            attempts++;
        }

        Debug.Log($"Spawned {spawnedPositions.Count} potions after {attempts} attempts");
    }

    Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * searchRadius;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, searchRadius, NavMesh.AllAreas))
        {
            Vector3 spawnPoint = hit.position;
            spawnPoint.y = 6f;
            return spawnPoint;
        }

        return Vector3.zero;
    }

    bool IsValidSpawnPoint(Vector3 position, List<Vector3> existingPositions)
    {
        foreach (Vector3 existingPos in existingPositions)
        {
            if (Vector3.Distance(position, existingPos) < minDistanceBetweenPotions)
            {
                return false;
            }
        }
        return true;
    }
}

public class PotionAnimation : MonoBehaviour
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

public class CollectablePotion : MonoBehaviour
{
    private Potions potionsInstance;

    public void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            if (potionsInstance == null)
            {
                potionsInstance = FindObjectOfType<Potions>();
            }
            if (potionsInstance.mainCamera.GetComponent<yarab>().getPotions() < 3)
            {
                potionsInstance.mainCamera.GetComponent<yarab>().IncreasePotions();

                // Destroy the potion object
                Destroy(gameObject);
            }
            
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

        // Initialize potionsInstance
        potionsInstance = FindObjectOfType<Potions>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Make it kinematic if you don't want it to be affected by physics
        }
    }
}
