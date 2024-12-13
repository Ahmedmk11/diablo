using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class lilithphase2testingscript : MonoBehaviour
{

    // lilith parameters
    public int health = 50;
    public int shield = 50;
    bool hasShield = true;
    bool hasReflectiveAura = false;



    Animator phase2Anim;
    public Light halo;
    public Light aura;
   //  public Transform playerPos;

    public GameObject particleSystem;
    public AnimatorController phase2controller;
    public Camera camera;



    private ParticleSystem particleSystemInstance;
    private float distanceInFront = 8.0f;

    private bool isUpdateDelayed = true;
    private bool waitBetweenCycles = true;

    private bool isStunned = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().runtimeAnimatorController = phase2controller;
        phase2Anim = GetComponent<Animator>();

        // make the pos of halo and aura the same as the pos of lilith
        halo.transform.position = transform.position;
        aura.transform.position = transform.position;
        halo.transform.parent = transform.GetChild(1);
        aura.transform.parent = transform.GetChild(1);
        halo.transform.localPosition = new Vector3(0, 0.5f, 0);
        aura.transform.localPosition = new Vector3(0, 0.5f, 0);




        particleSystemInstance = Instantiate(particleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particleSystemInstance.transform.Rotate(90, 0, 0);
        particleSystemInstance.gameObject.SetActive(false);

        aura.enabled = false;

        StartCoroutine(DelayUpdateLogic(10f));
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdateDelayed)
            return;

        if(hasShield)
        {
            halo.enabled = true;
        } else
        {
            halo.enabled = false;
        }

        if(!hasReflectiveAura && waitBetweenCycles && !isStunned)
        {
            phase2Anim.SetTrigger("reflective aura");
            StartCoroutine(ActivateAuraWithDelay());
            hasReflectiveAura = true;
            StartCoroutine(timeToDeactivateAura());
        }

        /*if (Input.GetKeyDown(KeyCode.S))
        {
            phase2Anim.SetTrigger("stun");
        }*/

      /*  if (Input.GetKeyDown(KeyCode.B))
        {
            phase2Anim.SetTrigger("blood spikes");

            StartCoroutine(ActivateParticleSystemWithDelay());
        }*/

      /*  if (Input.GetKeyDown(KeyCode.L))
        {
            halo.enabled = !halo.enabled;
        }*/

    }

    IEnumerator DelayUpdateLogic(float delay)
    {
        yield return new WaitForSeconds(delay);
        isUpdateDelayed = false; 
    }

    public void takeDamage(int damage)
    {
        if (hasReflectiveAura)
        {
            camera.GetComponent<yarab>().takeDamage(damage + 15);
        }
        else if (hasShield)
        {
            phase2Anim.SetTrigger("hitReaction");
            if (damage > shield)
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

    public void takeStun()
    {
        phase2Anim.SetTrigger("stun");
        isStunned = true;
        StartCoroutine(timeToUnstun());
    }

    IEnumerator timeToUnstun()
    {
        yield return new WaitForSeconds(5f);
        isStunned = false;
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

        // wahwah
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, 5f);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                camera.GetComponent<yarab>().takeDamage(30);
                Debug.Log($"Damaged enemy: {hit.name}");
            }
        }
    }

    IEnumerator timeToDeactivateAura()
    {
        yield return new WaitForSeconds(10f);
        hasReflectiveAura = false;
        aura.enabled = false;

        phase2Anim.SetTrigger("blood spikes");

        StartCoroutine(ActivateParticleSystemWithDelay());
        StartCoroutine(waitBetweenCyclesFunc());
    }

    IEnumerator waitBetweenCyclesFunc()
    {
        waitBetweenCycles = false;
        yield return new WaitForSeconds(10f);
        waitBetweenCycles = true;
    }

    IEnumerator ActivateAuraWithDelay()
    {
        // Delay for a second
        yield return new WaitForSeconds(1.3f);
        aura.enabled = true;
        hasReflectiveAura = true;
    }

    IEnumerator timeToActivateShield()
    {
        yield return new WaitForSeconds(10f);
        hasShield = true;
        shield = 50;
    }
}
