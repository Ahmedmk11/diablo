using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class yarab : MonoBehaviour
{
    public int level;
    private bool isDead = false;


    public GameObject barb;
    public GameObject sorc;
    public GameObject rogue;
    public GameObject lilith;
    public GameObject minion;

    public GameObject barbShield;

    public GameObject arrow1;
    public GameObject shower;
    public GameObject smoke;
    public GameObject spark;

    public Light lilithShield;
    public Light lilithAura;
    public GameObject bloodySpikes;

    public AnimatorController barbAnimator;
    public AnimatorController sorcAnimator;
    public AnimatorController rogueAnimator;
    public AnimatorController lilithAnimatorPhase1;
    public AnimatorController lilithAnimatorPhase2;
    public AnimatorController minionAnimator;


    public Movement movement;
    public Camera camera;
    public Camera minimapCamera;
    public GameObject marker;

    public GameObject currentCharacter;
    private string currentCharacterName;
    public GameObject currentBoss;

    public Animator animator;

    public Barbarian_Abilities Barbarian_Abilities;
    public LilithBehavior LilithPhase1;
    public lilithphase2testingscript Lilithphase2;
    public SorcererManager SorcererManager;

    public GameObject fireball;
    public GameObject clone;
    public GameObject inferno;
    public AnimatorController cloneAnimator;

    public PlayerAnimationTrigger playerAnimationTrigger;
    public ArrowShower arrowShower;
    public Bomb bomb;
    public Dash dash;

    public int maxHealth = 100;
    public int health = 100;
    public int xp = 0;
    public int characterLevel = 1;
    public int abilityPoints = 0;
    private int potions = 0;
    private int runes = 0;
    private bool invincible = false;

    public bool defensiveAbilityLocked = true;
    public bool wildacrdAbilityLocked = true;
    public bool ultimateAbilityLocked = true;
    private bool unlockAbilityCheat = false;

    public bool resetCooldowns = false;

    public bool enteredPhase2 = false;
    public bool enteredPhase2ForUI = false;
    private bool stopRotation = false;

    public GameObject pauseCanvas;
    GameObject canvasObject;

    // Start is called before the first frame update
    void Start()
    {
        print("yarab start");
        // TEMP
        string sceneName = SceneManager.GetActiveScene().name;

        // Set the level based on the scene
        if (sceneName == "Demo Blue")
        {
            level = 2;
        }
        else if (sceneName == "Dock Thing 1")
        {
            level = 1;
        }
        // ha5od variable men character selection screen 1: barb 2: sorc 3: rogue

        int character = CharacterSelection.count;
        // character = 3;
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
            currentCharacter.AddComponent<SorcererManager>();
            currentCharacter.GetComponent<SorcererManager>().camera = camera;
            currentCharacter.GetComponent<SorcererManager>().fireball = fireball;
            currentCharacter.GetComponent<SorcererManager>().clonePrefab = clone;
            currentCharacter.GetComponent<SorcererManager>().infernoPrefab = inferno;
            currentCharacter.GetComponent<SorcererManager>().cloneAnimator = cloneAnimator;
        }
        else if (character == 3)
        {
            currentCharacterName = "Rogue";
            currentCharacter = Instantiate(rogue, initVector, Quaternion.identity);
            currentCharacter.GetComponent<Animator>().runtimeAnimatorController = rogueAnimator;
            currentCharacter.AddComponent<PlayerAnimationTrigger>();
            currentCharacter.GetComponent<PlayerAnimationTrigger>().camera = camera;
            currentCharacter.GetComponent<PlayerAnimationTrigger>().arrowPrefab = arrow1;
            currentCharacter.GetComponent<PlayerAnimationTrigger>().arrowSpawnPoint = currentCharacter.transform.GetChild(4).transform;
            currentCharacter.GetComponent<PlayerAnimationTrigger>().launchForce = 17;
            currentCharacter.GetComponent<PlayerAnimationTrigger>().arrowLifetime = 5;
        }
        gameObject.GetComponent<CameraFollow>().target = currentCharacter.transform;

        currentCharacter.AddComponent<BoxCollider>();
        currentCharacter.GetComponent<BoxCollider>().center = new Vector3(0, 2, 0);
        currentCharacter.GetComponent<BoxCollider>().size = new Vector3(1, 2.5f, 1);
        currentCharacter.AddComponent<NavMeshAgent>();
        currentCharacter.AddComponent(movement.GetComponent<Movement>().GetType());
        currentCharacter.GetComponent<Movement>().camera = camera;
        animator = currentCharacter.GetComponent<Animator>();
        animator.applyRootMotion = false;
        currentCharacter.tag = "Player";

        if (level == 1)
        {
            minimapCamera.AddComponent<CameraFollow>();
            minimapCamera.GetComponent<CameraFollow>().target = marker.transform;
            minimapCamera.GetComponent<CameraFollow>().positionOffset = new Vector3(0, 10, -5);
            minimapCamera.GetComponent<CameraFollow>().rotationY = 0;

            GetComponent<CreateCamp>().player = currentCharacter.transform;
        }
        else if (level == 2)
        {
            print("level 2");
            currentBoss = Instantiate(lilith, new Vector3(-36.8f, 4.02f, 37.6f), Quaternion.identity);
            currentBoss.AddComponent<BoxCollider>();
            // raise and stretch the box collider
            currentBoss.GetComponent<BoxCollider>().center = new Vector3(0, 2, 0);
            currentBoss.GetComponent<BoxCollider>().size = new Vector3(1, 4, 1);
            currentBoss.tag = "Enemy";
            currentBoss.AddComponent<LilithBehavior>();
            currentBoss.GetComponent<Animator>().runtimeAnimatorController = lilithAnimatorPhase1;
            currentBoss.GetComponent<LilithBehavior>().animatorController = lilithAnimatorPhase1;
            currentBoss.GetComponent<LilithBehavior>().minionPrefab = minion;
            currentBoss.GetComponent<LilithBehavior>().cameraForYarab = camera;
            currentBoss.GetComponent<LilithBehavior>().player = currentCharacter;
            currentBoss.GetComponent<LilithBehavior>().minionController = minionAnimator;
            currentBoss.GetComponent<Animator>().applyRootMotion = false;
        }
        canvasObject = GameObject.Find("Canvas");

        if (level == 2 && PlayerPrefs.GetInt("level") != 69)
        {
            maxHealth = PlayerPrefs.GetInt("maxHealth");
            health = PlayerPrefs.GetInt("health");
            xp = PlayerPrefs.GetInt("xp");
            characterLevel = PlayerPrefs.GetInt("characterLevel");
            abilityPoints = PlayerPrefs.GetInt("abilityPoints");
            potions = PlayerPrefs.GetInt("potions");
            runes = PlayerPrefs.GetInt("runes");
            defensiveAbilityLocked = PlayerPrefs.GetInt("defensiveAbilityLocked") == 1;
            wildacrdAbilityLocked = PlayerPrefs.GetInt("wildacrdAbilityLocked") == 1;
            ultimateAbilityLocked = PlayerPrefs.GetInt("ultimateAbilityLocked") == 1;
            unlockAbilityCheat = PlayerPrefs.GetInt("unlockAbilityCheat") == 1;
        }

    }
    // private bool on = true; // remove
    // Update is called once per frame
    void Update()
    {
        // make the marker follow the player and rotate with the player about y axis
        if (level == 1)
        {
            marker.transform.position = new Vector3(currentCharacter.transform.position.x, 15, currentCharacter.transform.position.z - 5);
            marker.transform.rotation = Quaternion.Euler(90, currentCharacter.transform.rotation.eulerAngles.y, 0);
        }


        if(currentCharacter.transform.position.x >= 75 && currentCharacter.transform.position.x <= 80
            && currentCharacter.transform.position.z >= 35 && currentCharacter.transform.position.z <= 40
            && level == 1 && runes >= 3
          )
        {
            print("You passed level 1");
            // go to boss level
            PlayerPrefs.SetInt("maxHealth", maxHealth);
            PlayerPrefs.SetInt("health", health);
            PlayerPrefs.SetInt("xp", xp);
            PlayerPrefs.SetInt("characterLevel", characterLevel);
            PlayerPrefs.SetInt("abilityPoints", abilityPoints);
            PlayerPrefs.SetInt("potions", potions);
            PlayerPrefs.SetInt("runes", runes - 3);
            PlayerPrefs.SetInt("defensiveAbilityLocked", defensiveAbilityLocked ? 1 : 0);
            PlayerPrefs.SetInt("wildacrdAbilityLocked", wildacrdAbilityLocked ? 1 : 0);
            PlayerPrefs.SetInt("ultimateAbilityLocked", ultimateAbilityLocked ? 1 : 0);
            PlayerPrefs.SetInt("unlockAbilityCheat", unlockAbilityCheat ? 1 : 0);

            SceneManager.LoadScene("Demo Blue");
        }

        if (level == 2 && !stopRotation)
        {
            currentBoss.transform.LookAt(currentCharacter.transform);
        }

        if(currentCharacterName == "Sorceress" && !currentCharacter.GetComponent<SorcererManager>().WildcardAbilityLockedSorc && level == 1)
        {
            if(currentCharacter.GetComponent<SorcererManager>().cloneActive)
            {
                GetComponent<CreateCamp>().changePlayerToFollow(currentCharacter.GetComponent<SorcererManager>().clonePosition);
            }
            else
            {
                GetComponent<CreateCamp>().changePlayerToFollow(currentCharacter.transform);
            }
        }
        if(currentCharacterName == "Sorceress" && !currentCharacter.GetComponent<SorcererManager>().WildcardAbilityLockedSorc && level == 2 && !enteredPhase2)
        {
            if (currentCharacter.GetComponent<SorcererManager>().cloneActive)
            {
                currentBoss.GetComponent<LilithBehavior>().changePlayerToFollow(currentCharacter.GetComponent<SorcererManager>().clonePosition);
            }
            else
            {
                currentBoss.GetComponent<LilithBehavior>().changePlayerToFollow(currentCharacter.transform);
            }
        }

        if (level == 2 && !enteredPhase2 && currentBoss.GetComponent<LilithBehavior>().health <= 0) //entering phase 2
        {
            stopRotation = true;
            enteredPhase2 = true;
            StartCoroutine(WaitForLilithToDie());
        }

        if(enteredPhase2ForUI && currentBoss.GetComponent<lilithphase2testingscript>().health <= 0)
        {
            print("Lilith is dead");
            // game over you won screen
            SceneManager.LoadScene("win screen");
        }

        if (characterLevel < 4)
        {
            if (xp >= 100 * characterLevel)
            {
                xp -= (100 * characterLevel);
                characterLevel++;
                maxHealth = characterLevel * 100;
                health = maxHealth;
                abilityPoints++;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (potions > 0 && health < maxHealth)
            {
                animator.SetTrigger("heal");
                StartCoroutine(WaitForAnimationToHeal(0.5f));
            }
        }

        // Add abilities for rogue
        if (currentCharacterName == "Rogue")
        {
            if (!defensiveAbilityLocked)
            {
                if(!unlockAbilityCheat) abilityPoints--;
                defensiveAbilityLocked = true;
                currentCharacter.AddComponent<Bomb>();
                currentCharacter.GetComponent<Bomb>().arrowPrefab = smoke;
                currentCharacter.GetComponent<Bomb>().camera = camera;
                currentCharacter.GetComponent<Bomb>().arrowSpawnPoint = currentCharacter.transform.GetChild(1).transform;
                currentCharacter.GetComponent<Bomb>().smoke = spark;
                currentCharacter.GetComponent<Bomb>().launchForce = 7;
                currentCharacter.GetComponent<Bomb>().arrowLifetime = 5;
                currentCharacter.GetComponent<Bomb>().detectionRadius = 5;
            }
            if (!wildacrdAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                wildacrdAbilityLocked = true;
                currentCharacter.AddComponent<Dash>();
                currentCharacter.GetComponent<Dash>().camera = camera;
            }
            if (!ultimateAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                ultimateAbilityLocked = true;
                currentCharacter.AddComponent<ArrowShower>();
                currentCharacter.GetComponent<ArrowShower>().arrowPrefab = shower;
                currentCharacter.GetComponent<ArrowShower>().camera = camera;
                currentCharacter.GetComponent<ArrowShower>().launchForce = 20;
                currentCharacter.GetComponent<ArrowShower>().arrowLifetime = 5;
                currentCharacter.GetComponent<ArrowShower>().detectionRadius = 2;
            }
        }
        else if (currentCharacterName == "Barbarian")
        {
            if (!defensiveAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                defensiveAbilityLocked = true;
                currentCharacter.GetComponent<Barbarian_Abilities>().DefensiveAbilityLockedBarb = false;
            }
            if (!wildacrdAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                wildacrdAbilityLocked = true;
                currentCharacter.GetComponent<Barbarian_Abilities>().WildcardAbilityLockedBarb = false;
            }
            if (!ultimateAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                ultimateAbilityLocked = true;
                currentCharacter.GetComponent<Barbarian_Abilities>().UltimateAbilityLockedBarb = false;
            }
        }
        else if(currentCharacterName == "Sorceress")
        {
            if (!defensiveAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                defensiveAbilityLocked = true;
                currentCharacter.GetComponent<SorcererManager>().DefensiveAbilityLockedSorc = false;
            }
            if (!wildacrdAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                wildacrdAbilityLocked = true;
                currentCharacter.GetComponent<SorcererManager>().WildcardAbilityLockedSorc = false;
            }
            if (!ultimateAbilityLocked)
            {
                if (!unlockAbilityCheat) abilityPoints--;
                ultimateAbilityLocked = true;
                currentCharacter.GetComponent<SorcererManager>().UltimateAbilityLockedSorc = false;
            }
        }
    
        // cheats
        if (Input.GetKeyDown(KeyCode.H))
        {
            health += 20;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            health -= 20;
            if (health <= 0 && !isDead)
            {
                health = 0;
                isDead = true;
                StartCoroutine(WaitForAnimationToDie(5));

            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            invincible = !invincible;
            print("invincible: " + invincible);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            abilityPoints++;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            gainXP(100);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.5f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            defensiveAbilityLocked = false;
            wildacrdAbilityLocked = false;
            ultimateAbilityLocked = false;
            unlockAbilityCheat = true;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            resetCooldowns = true;
        }
        if (Input.GetKeyDown(KeyCode.R)) // remove
        {
            runes++;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause();
        }
    }

    public void pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        pauseCanvas.active = !pauseCanvas.active;
        canvasObject.active = !canvasObject.active;
    }

    private IEnumerator WaitForLilithToDie()
    {
        yield return new WaitForSeconds(10);
        StartCoroutine(StopRotation());
        currentBoss.GetComponent<Animator>().runtimeAnimatorController = lilithAnimatorPhase2;
        LilithBehavior lb = currentBoss.GetComponent<LilithBehavior>();
        Destroy(lb);
        currentBoss.AddComponent<lilithphase2testingscript>();
        currentBoss.GetComponent<lilithphase2testingscript>().phase2controller = lilithAnimatorPhase2;
        currentBoss.GetComponent<lilithphase2testingscript>().halo = lilithShield;
        currentBoss.GetComponent<lilithphase2testingscript>().aura = lilithAura;
        currentBoss.GetComponent<lilithphase2testingscript>().particleSystem = bloodySpikes;
        currentBoss.GetComponent<lilithphase2testingscript>().camera = camera;
        enteredPhase2ForUI = true;
    }

    private IEnumerator StopRotation()
    {
        yield return new WaitForSeconds(5);
        stopRotation = false;
    }

    private IEnumerator WaitForAnimationToHeal(float v)
    {
        yield return new WaitForSeconds(v);
        potions--;
        health += maxHealth / 2;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void takeDamage(int damage, string attacker = "Enemy", float clipLength = 0)
    {
        if (invincible)
        {
            return;
        }
        if (currentCharacterName == "Barbarian")
        {
            if (currentCharacter.GetComponent<Barbarian_Abilities>().shieldActive == true) 
                return;
        }
        if(currentCharacterName == "Sorceress")
        {
            if (currentCharacter.GetComponent<SorcererManager>().cloneActive == true)
                return;
        }
        
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }

        if (!isDead && health <= 0)
        {
            isDead = true;
            StartCoroutine(WaitForAnimationToDie(5)); 
            Debug.Log("took damage from " + attacker + " and died");
        }
        else
        {
            StartCoroutine(WaitForAnimationToGetHit(clipLength)); 
            Debug.Log("took damage from " + attacker + " " + health);
        }
    }

    private IEnumerator WaitForAnimationToDie(float clipLength)
    {
        animator.SetTrigger("death");
        yield return new WaitForSeconds(clipLength);
        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator WaitForAnimationToGetHit(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        if (health > 0) 
        {
            animator.SetTrigger("hit reaction");
        }
    }

    public int gainXP(int xp)
    {
        if (characterLevel < 4)
        {
            this.xp += xp;
            return this.xp;
        }
        return 0;
    }

    public int getPotions()
    {
        return this.potions;
    }

    public int getRunes()
    {
        return this.runes;
    }

    public int IncreasePotions()
    {
        this.potions += 1;
        return this.potions;
    }

    public int IncreaseRunes()
    {
        this.runes += 1;
        return this.runes;
    }
}
