using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject xpBar;
    [SerializeField] private GameObject levelText;
    [SerializeField] private List<GameObject> abilities;
    [SerializeField] private GameObject potions;
    [SerializeField] private GameObject abilityPoints;
    [SerializeField] private GameObject runes;
    [SerializeField] private GameObject camera;

    [SerializeField] private GameObject bossHealth;
    [SerializeField] private GameObject bossShield;
    private bool first = true;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        IncreaseXP();
        IncreaseItemCount("Potion");
        IncreaseItemCount("Ability Point");
        IncreaseItemCount("Rune");
        if(camera.GetComponent<yarab>().level == 2)
        {
            updateBossHealth();
        }
    }
    void UpdateHealth()
    {
        Transform fill = healthBar.transform.Find("fill");
        TMP_Text text = healthBar.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        int health = int.Parse(text.text);
        fill.GetComponent<Image>().fillAmount = (float) ((float)camera.GetComponent<yarab>().health / (float)camera.GetComponent<yarab>().maxHealth);
        text.text = camera.GetComponent<yarab>().health.ToString();
    }

    void IncreaseXP()
    {
        // Assuming xpBar has a Slider component
        Slider slider = xpBar.GetComponent<Slider>();

        TMP_Text xpText = xpBar.transform.Find("XP Text").GetComponent<TMP_Text>();
        xpText.text = (camera.GetComponent<yarab>().xp).ToString() + "/" + 100 * camera.GetComponent<yarab>().characterLevel;
        slider.value = camera.GetComponent<yarab>().xp / (100f * camera.GetComponent<yarab>().characterLevel);
        
        
        int level = camera.GetComponent<yarab>().characterLevel;
        TMP_Text tMP_Text = levelText.GetComponent<TMP_Text>();
        tMP_Text.text = level.ToString();


       
        
    }

    void IncreaseItemCount(string type)
    {
        TMP_Text text = null;
        if (type == "Potion")
        {
            text = potions.GetComponent<TMP_Text>();
            text.text = camera.GetComponent<yarab>().getPotions().ToString();
        }
        else if (type == "Ability Point")
        {
            text = abilityPoints.GetComponent<TMP_Text>();
            text.text = camera.GetComponent<yarab>().abilityPoints.ToString();
        }
        else if (type == "Rune")
        {
            text = runes.GetComponent<TMP_Text>();
            text.text = camera.GetComponent<yarab>().getRunes().ToString();
        }
    }

    void updateBossHealth()
    {
        if(!camera.GetComponent<yarab>().enteredPhase2ForUI)
        {
            Slider slider = bossHealth.GetComponent<Slider>();
            TMP_Text text = bossHealth.transform.Find("HP Text").GetComponent<TMP_Text>();
            int health = int.Parse(text.text);
            slider.value = (float)((float)camera.GetComponent<yarab>().currentBoss.GetComponent<LilithBehavior>().health / 50f);
            text.text = camera.GetComponent<yarab>().currentBoss.GetComponent<LilithBehavior>().health.ToString();
        }
        else
        {
            if (first)
            {
                first = false;
                bossShield.SetActive(true);
            }
            Slider slider = bossHealth.GetComponent<Slider>();
            TMP_Text text = bossHealth.transform.Find("HP Text").GetComponent<TMP_Text>();
            slider.value = (float)((float)camera.GetComponent<yarab>().currentBoss.GetComponent<lilithphase2testingscript>().health / 50f);
            text.text = camera.GetComponent<yarab>().currentBoss.GetComponent<lilithphase2testingscript>().health.ToString();

            Slider slider2 = bossShield.GetComponent<Slider>();
            slider2.value = (float)((float)camera.GetComponent<yarab>().currentBoss.GetComponent<lilithphase2testingscript>().shield / 50f);
        }


    
        
        
    }
}
