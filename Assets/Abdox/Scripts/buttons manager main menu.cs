using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonsmanagermainmenu : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button selectLevel;
    [SerializeField] private Button options;
    [SerializeField] private Button exit;



    // Start is called before the first frame update
    void Start()
    {
        newGame.onClick.AddListener(() => newGameClick());
        selectLevel.onClick.AddListener(() => selectLevelClick());
        options.onClick.AddListener(() => optionsClick());
        exit.onClick.AddListener(() => exitClick());
    }

    private void newGameClick()
    {
        SceneManager.LoadScene("CharChoice");
    }

    private void selectLevelClick()
    {
        SceneManager.LoadScene("SelectLevel");
    }

    private void optionsClick()
    {
        SceneManager.LoadScene("OptionsMenus");
    }

    private void exitClick()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
