using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class yarab : MonoBehaviour
{
    private int level;



    public GameObject barb;
    public GameObject sorc;
    public GameObject rogue;
    public GameObject lilith;

    public GameObject barbShield;

    public AnimatorController barbAnimator;
    public AnimatorController sorcAnimator;
    public AnimatorController rogueAnimator;


    public Movement movement;
    public Camera camera;
    public Camera minimapCamera;
    public GameObject marker;

    private GameObject currentCharacter;
    private string currentCharacterName;
    private GameObject currentBoss;

    private Animator animator;

    public Barbarian_Abilities Barbarian_Abilities;


    // Start is called before the first frame update
    void Start()
    {
        // TEMP
        level = 2;
        // ha5od variable men character selection screen 1: barb 2: sorc 3: rogue
        int character = 1;
        // TEMP

        //Vector3 initVector = level == 1 ? new Vector3(-3.41f, 5, -25.5f) : new Vector3(50, 50, 50);
        //Vector3 initVector = level == 1 ? new Vector3(-3.41f, 5, -25.5f) : new Vector3(-36.8f, 0.81f, -10.6f);
        Vector3 initVector = level == 1 ? new Vector3(-3.41f, 5, -25.5f) : new Vector3(-36.8f, 4.71f, 17.6f);


        if (character == 1)
        {
            currentCharacterName = "Barbarian";
            currentCharacter = Instantiate(barb, initVector, Quaternion.identity);
            currentCharacter.GetComponent<Animator>().runtimeAnimatorController = barbAnimator;
            currentCharacter.AddComponent<Barbarian_Abilities>();
            currentCharacter.GetComponent<Barbarian_Abilities>().camera = camera;
            currentCharacter.GetComponent<Barbarian_Abilities>().shieldLight = barbShield;
            barbShield.transform.parent = currentCharacter.transform.GetChild(1);
            barbShield.transform.localPosition = new Vector3(0, 0.5f, 0);

        }
        else if (character == 2)
        {
            currentCharacterName = "Sorceress";
            currentCharacter = Instantiate(sorc, initVector, Quaternion.identity);
            currentCharacter.GetComponent<Animator>().runtimeAnimatorController = sorcAnimator;
        }
        else if (character == 3)
        {
            currentCharacterName = "Rogue";
            currentCharacter = Instantiate(rogue, initVector, Quaternion.identity);
            currentCharacter.GetComponent<Animator>().runtimeAnimatorController = rogueAnimator;
        }
        gameObject.GetComponent<CameraFollow>().target = currentCharacter.transform;

        currentCharacter.AddComponent<NavMeshAgent>();
        currentCharacter.AddComponent(movement.GetComponent<Movement>().GetType());
        currentCharacter.GetComponent<Movement>().camera = camera;
        animator = currentCharacter.GetComponent<Animator>();
        animator.applyRootMotion = false;
        minimapCamera.AddComponent<CameraFollow>();
        minimapCamera.GetComponent<CameraFollow>().target = marker.transform;

        if (level == 1)
        {
            // add minions / demons
        } 
        else if (level == 2)
        {
            currentBoss = Instantiate(lilith, new Vector3(-36.8f, 4.02f, 37.6f), Quaternion.identity);
            currentBoss.AddComponent<BoxCollider>();
            // raise and stretch the box collider
            currentBoss.GetComponent<BoxCollider>().center = new Vector3(0, 2, 0);
            currentBoss.GetComponent<BoxCollider>().size = new Vector3(1, 4, 1);
            currentBoss.tag = "Enemy";
            //animator.applyRootMotion = false;

        }


    }

    // Update is called once per frame
    void Update()
    {
        // make the marker follow the player and rotate with the player about y axis
        marker.transform.position = new Vector3(currentCharacter.transform.position.x, 15, currentCharacter.transform.position.z - 5);
        marker.transform.rotation = Quaternion.Euler(90, currentCharacter.transform.rotation.eulerAngles.y, 0);

        if (level == 2)
        {
            currentBoss.transform.LookAt(currentCharacter.transform);
        }

    }
}
