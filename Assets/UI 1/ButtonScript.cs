using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private int cooldown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        // Disable the button
        GetComponent<UnityEngine.UI.Button>().interactable = false;

        // Get the fill GameObject and set its fill amount to 0
        Transform fill = transform.Find("Fill");
        UnityEngine.UI.Image fillImage = fill.GetComponent<UnityEngine.UI.Image>();
        fillImage.fillAmount = 0;

        // Increment the fill amount over the cooldown period
        float elapsed = 0;
        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = elapsed / cooldown;
            yield return null;
        }

        // Re-enable the button
        GetComponent<UnityEngine.UI.Button>().interactable = true;
    }
}
