using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectlevel : MonoBehaviour
{
    [SerializeField] private Button back;
    [SerializeField] private Button baseLevel;
    [SerializeField] private Button boss;
    // Start is called before the first frame update
    void Start()
    {
        back.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });
        baseLevel.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharChoice");
        });
        boss.onClick.AddListener(() =>
        {
            CharacterSelection.FromSelectLevel = 2;
            PlayerPrefs.SetInt("level", 69);
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharChoice");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
