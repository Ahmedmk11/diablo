using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    private int count=0;
    public GameObject barbarian;
    public GameObject sorcer;

    public GameObject rouge;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
       
        
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
