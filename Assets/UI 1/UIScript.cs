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
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            IncreaseHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            DecreaseHealth(10);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            IncreaseXP(10);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DecreaseXP(10);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            IncreaseItemCount(potions);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            DecreaseItemCount(potions);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            IncreaseItemCount(abilityPoints);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DecreaseItemCount(abilityPoints);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            IncreaseItemCount(runes);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            DecreaseItemCount(runes);
        }
    }
    void IncreaseHealth(int amount)
    {
        Transform fill = healthBar.transform.Find("fill");
        TMP_Text text = healthBar.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        int health = int.Parse(text.text);
        if (health < 100)
        {
            fill.GetComponent<Image>().fillAmount += amount / 100f;
            text.text = (int.Parse(text.text) + amount).ToString();
        }
    }

    void DecreaseHealth(int amount)
    {
        Transform fill = healthBar.transform.Find("fill");
        TMP_Text text = healthBar.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        int health = int.Parse(text.text);
        if (health > 0)
        {
            fill.GetComponent<Image>().fillAmount -= amount / 100f;
            text.text = (int.Parse(text.text) - amount).ToString();
        }
    }

    void IncreaseXP(int amount)
    {
        // Assuming xpBar has a Slider component
        Slider slider = xpBar.GetComponent<Slider>();
        if (slider.value < 1)
        {
            TMP_Text xpText = xpBar.transform.Find("XP Text").GetComponent<TMP_Text>();
            string currentXp = xpText.text.Split('/')[0];
            string xpToLevel = xpText.text.Split('/')[1];
            xpText.text = (int.Parse(currentXp) + amount).ToString() + "/" + xpToLevel;
            slider.value += amount / 100f;
        }
        if (slider.value == 1)
        {
            string levelTextContent = levelText.GetComponent<TMP_Text>().text;
            int level = int.Parse(levelTextContent);
            if (level < 4)
            {
                TMP_Text xpText = xpBar.transform.Find("XP Text").GetComponent<TMP_Text>();
                string currentXp = xpText.text.Split('/')[0];
                string xpToLevel = xpText.text.Split('/')[1];
                level++;
                levelText.GetComponent<TMP_Text>().text = level.ToString();
                xpText.text = "0/" + 100 * level;
                slider.value = 0;
            }
        }
    }

    void DecreaseXP(int amount)
    {
        // Assuming xpBar has a Slider component
        Slider slider = xpBar.GetComponent<Slider>();
        if (slider.value > 0)
        {
            TMP_Text xpText = xpBar.transform.Find("XP Text").GetComponent<TMP_Text>();
            string currentXp = xpText.text.Split('/')[0];
            string xpToLevel = xpText.text.Split('/')[1];
            xpText.text = (int.Parse(currentXp) - amount).ToString() + "/" + xpToLevel;
            slider.value -= amount / 100f;
        }
        if (slider.value == 0)
        {
            TMP_Text levelTextComponent = levelText.GetComponent<TMP_Text>();
            int level = int.Parse(levelTextComponent.text);
            if (level > 1)
            {
                TMP_Text xpText = xpBar.transform.Find("XP Text").GetComponent<TMP_Text>();
                string currentXp = xpText.text.Split('/')[0];
                string xpToLevel = xpText.text.Split('/')[1];
                level--;
                levelTextComponent.text = level.ToString();
                xpText.text = "90/" + 100 * level;
                slider.value = 0.9f;
            }
        }
    }

    void IncreaseItemCount(GameObject item)
    {
        TMP_Text text = item.GetComponent<TMP_Text>();
        if (text != null)
        {
            int count = int.Parse(text.text);
            count++;
            text.text = count.ToString();
        }
    }

    void DecreaseItemCount(GameObject item)
    {
        TMP_Text text = item.GetComponent<TMP_Text>();
        if (text != null)
        {
            int count = int.Parse(text.text);
            if (count > 0)
            {
                count--;
                text.text = count.ToString();
            }
        }
    }
}
