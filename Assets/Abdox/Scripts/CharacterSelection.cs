using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public static int count=1;
    public static int FromSelectLevel = 0;
    public GameObject barbarian;
    public GameObject sorcer;

    public GameObject rouge;

    public GameObject barbText;
    public GameObject rogueText;
    public GameObject archerText;

     public GameObject barbAb;
    public GameObject rogueAb;
    public GameObject archerAb;

    [SerializeField] private Button Select;
    [SerializeField] private Button back;
    

    


    // Start is called before the first frame update
    void Start()
    {
        sorcer.SetActive(false);
        barbarian.SetActive(false);
        rouge.SetActive(false);
        barbText.SetActive(false);
        rogueText.SetActive(false);
        archerText.SetActive(false);
         barbAb.SetActive(false);
        rogueAb.SetActive(false);
        archerAb.SetActive(false);

        Select.onClick.AddListener(() => SelectClick());
        back.onClick.AddListener(() => backClick());
        
    }

    private void SelectClick()
    {
        if (FromSelectLevel == 2) UnityEngine.SceneManagement.SceneManager.LoadScene("Demo Blue");
        else UnityEngine.SceneManagement.SceneManager.LoadScene("Dock Thing 1");
    }

    private void backClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        count = 1;
    }

    // Update is called once per frame
    void Update()
    {
        turnCharacters(count);
       
        
    }

    public void turnCharacters(int c){
        if(c==1){
            sorcer.SetActive(false);
        barbarian.SetActive(true);
        rouge.SetActive(false);
        barbText.SetActive(true);
        rogueText.SetActive(false);
        archerText.SetActive(false);
          barbAb.SetActive(true);
        rogueAb.SetActive(false);
        archerAb.SetActive(false);
        }
        else if(c==2){
            sorcer.SetActive(true);
        barbarian.SetActive(false);
        rouge.SetActive(false);
        barbText.SetActive(false);
        rogueText.SetActive(true);
        archerText.SetActive(false);
          barbAb.SetActive(false);
        rogueAb.SetActive(true);
        archerAb.SetActive(false);
        }
        else {
            sorcer.SetActive(false);
        barbarian.SetActive(false);
        rouge.SetActive(true);
        barbText.SetActive(false);
        rogueText.SetActive(false);
        archerText.SetActive(true);
          barbAb.SetActive(false);
        rogueAb.SetActive(false);
        archerAb.SetActive(true);
        }
    }
    public void right(){
            if(count==3){
                count=1;
            }else{
                count++;
            }
             Debug.Log(count);
        }
        public void left(){
            if(count==1){
                count=3;
            }else{
                count--;
            }
             Debug.Log(count);
        }
}
