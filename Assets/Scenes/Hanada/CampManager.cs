using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CampManager : MonoBehaviour
{
    public Transform player;
    public int maxDemonsAlerted = 1;
    public int maxMinionsAlerted = 5;
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
            demon.agent.SetDestination(demonPositions[demon]);
            demon.StopFollowingPlayer();
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
