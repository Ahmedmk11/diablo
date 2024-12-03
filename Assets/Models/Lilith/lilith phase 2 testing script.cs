using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lilithphase2testingscript : MonoBehaviour
{

    // lilith parameters
    int health = 50;
    int shield = 50;
    bool hasShield = true;
    bool hasReflectiveAura = false;



    Animator phase2Anim;
    public Light halo;
    public Light aura;
    public Transform playerPos;

    public GameObject particleSystem;
    private ParticleSystem particleSystemInstance;
    private float distanceInFront = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        phase2Anim = GetComponent<Animator>();

        particleSystemInstance = Instantiate(particleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particleSystemInstance.transform.Rotate(90, 0, 0);
        particleSystemInstance.gameObject.SetActive(false);

        aura.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerPos);

        if(hasShield)
        {
            halo.enabled = true;
        } else
        {
            halo.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            phase2Anim.SetTrigger("stun");
        }

        if (Input.GetKeyDown(KeyCode.R)) // reflective aura funtionality
        {
            phase2Anim.SetTrigger("reflective aura");
            StartCoroutine(ActivateAuraWithDelay());
            hasReflectiveAura = true;
            StartCoroutine(timeToDeactivateAura());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            phase2Anim.SetTrigger("blood spikes");

            StartCoroutine(ActivateParticleSystemWithDelay());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            halo.enabled = !halo.enabled;
        }

    }

    public void takeDamage(int damage)
    {
        phase2Anim.SetTrigger("hitReaction");

        if (hasReflectiveAura)
        {
             // playerPos.GetComponent<PlayerController>().takeDamage(damage + 15); ????????????????????
        }
        else if (hasShield)
        {
            if(damage > shield)
            {
                int healthDamage = damage - shield;
                health -= healthDamage;
                if (health <= 0)
                {
                    phase2Anim.SetTrigger("death");
                }
                hasShield = false;
                shield = 0;
                StartCoroutine(timeToActivateShield());
            }
            else
            {
                shield -= damage;
                if (shield == 0)
                {
                    hasShield = false;
                    shield = 0;
                    StartCoroutine(timeToActivateShield());
                }
            }              
        }
        else
        {
            health -= damage;
            if (health <= 0)
            {
                phase2Anim.SetTrigger("death");
            }
        }
    }

    IEnumerator ActivateParticleSystemWithDelay()
    {
        // Delay for a second
        yield return new WaitForSeconds(1.3f);
        Quaternion rotation = Quaternion.Euler(90, 3, transform.rotation.eulerAngles.y);
        particleSystemInstance.transform.rotation = rotation;
        Vector3 spawnPosition = transform.position + transform.forward * distanceInFront;
        particleSystemInstance.transform.position = spawnPosition;
        particleSystemInstance.gameObject.SetActive(true);
        particleSystemInstance.Play();
    }

    IEnumerator timeToDeactivateAura()
    {
        yield return new WaitForSeconds(10f);
        hasReflectiveAura = false;
        aura.enabled = false;
    }

    IEnumerator ActivateAuraWithDelay()
    {
        // Delay for a second
        yield return new WaitForSeconds(1.3f);
        aura.enabled = true;
    }

    IEnumerator timeToActivateShield()
    {
        yield return new WaitForSeconds(10f);
        hasShield = true;
        shield = 50;
    }
}
