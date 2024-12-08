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
    private void Start()
    {
        float initialDistanceToCenter = Vector3.Distance(player.position, centerPoint);
        isPlayerInsideCampRadius = initialDistanceToCenter <= campRadius;
    }

    private void Update()
    {
        float distanceToCenter = Vector3.Distance(player.position, centerPoint);

        if (!isPlayerInsideCampRadius && distanceToCenter <= campRadius)
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

        if (!playerDied && demons[0].yarabScript.health <= 0)
        // if (!playerDied && playerHealth <= 0)
        {
            playerDied = true;
            ResetNearbyEntities();
            Debug.Log("Player died (testing reset)");
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

            if (alertedMinions.Count < maxMinionsAlerted && !isPlayerInsideCampRadius && distanceToCenter <= campRadius)
            {
                isPlayerInsideCampRadius = true;
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

        if (IsCampGenocided())
        {
            Debug.Log("What have I done?");
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

        if (alertedDemons.Count < maxDemonsAlerted && !isPlayerInsideCampRadius && distanceToCenter <= campRadius)
        {
            isPlayerInsideCampRadius = true;
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

        if (IsCampGenocided())
        {
            Debug.Log("What have I done?");
        }
    }

    private bool IsCampGenocided()
    {
        return demons.Count == 0 && minions.Count == 0;
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
