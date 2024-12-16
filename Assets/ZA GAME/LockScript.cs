using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockScript : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private Camera camera;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            cheatUnlock();
        }
    }

    public void level2unlocker()
    {
        GetComponentInParent<UnityEngine.UI.Button>().interactable = false; // Make the parent button interactable
        GetComponent<UnityEngine.UI.Button>().interactable = false; // Make this button non-interactable
        gameObject.SetActive(false); // Hide this component
    }

    public void OnUnlockButtonClick()
    {
       
        int count = camera.GetComponent<yarab>().abilityPoints;
        if (count > 0)
        {
            if (type == "defensive")
            {
                camera.GetComponent<yarab>().defensiveAbilityLocked = false;
            }
            else if (type == "wild card")
            {
                camera.GetComponent<yarab>().wildacrdAbilityLocked = false;
            }
            else if (type == "ultimate")
            {
                camera.GetComponent<yarab>().ultimateAbilityLocked = false;
            }
            GetComponentInParent<UnityEngine.UI.Button>().interactable = false; // Make the parent button interactable
            GetComponent<UnityEngine.UI.Button>().interactable = false; // Make this button non-interactable
            gameObject.SetActive(false); // Hide this component
        }
        
    }

    public void cheatUnlock()
    {
        GetComponentInParent<UnityEngine.UI.Button>().interactable = false; // Make the parent button interactable
        GetComponent<UnityEngine.UI.Button>().interactable = false; // Make this button non-interactable
        gameObject.SetActive(false); // Hide this component
    }
}
