using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fireball : MonoBehaviour
{
    [SerializeField] private LayerMask navMeshSurfaceLayer;
    [SerializeField] private float lifetime = 5f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            findAndDamageEnemy(5, collision.transform);
            Destroy(gameObject);
        }
        else if (IsNavMeshSurface(collision))
        {
            Destroy(gameObject);
        }
    }

    private bool IsNavMeshSurface(Collision collision)
    {
        return (navMeshSurfaceLayer.value & (1 << collision.gameObject.layer)) > 0;
    }

    private void findAndDamageEnemy(int damage, Transform target)
    {
        LilithBehavior lilith = target.GetComponent<LilithBehavior>();
        lilithphase2testingscript lilith2 = target.GetComponent<lilithphase2testingscript>();
        Minion minion = target.GetComponent<Minion>();
        Demon demon = target.GetComponent<Demon>();

        if (lilith != null)
        {
            if (damage == 100) damage = 20;
            lilith.takeDamage(damage);
        }
        else if (lilith2 != null)
        {
            if (damage == 100) damage = 20;
            lilith2.takeDamage(damage);
        }
        else if (minion != null)
        {
            minion.takeDamage(damage);
        }
        else if (demon != null)
        {
            demon.takeDamage(damage);
        }
    }
}
