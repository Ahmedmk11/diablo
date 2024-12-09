using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    private int count=1;
    public GameObject barbarian;
    public GameObject sorcer;

    public GameObject rouge;

    public GameObject barbText;
    public GameObject rogueText;
    public GameObject archerText;

     public GameObject barbAb;
    public GameObject rogueAb;
    public GameObject archerAb;

    

    


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
