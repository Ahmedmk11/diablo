using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockScript : MonoBehaviour
{
    [SerializeField] private GameObject abilityPoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnUnlockButtonClick()
    {
            TMP_Text text = abilityPoints.GetComponent<TMP_Text>();
            if (text != null)
            {
                int count = int.Parse(text.text);
                if (count > 0)
                {
                    count--;
                    text.text = count.ToString();
                    GetComponentInParent<UnityEngine.UI.Button>().interactable = true; // Make the parent button interactable
                    GetComponent<UnityEngine.UI.Button>().interactable = false; // Make this button non-interactable
                    gameObject.SetActive(false); // Hide this component
                }
            }
    }
}
