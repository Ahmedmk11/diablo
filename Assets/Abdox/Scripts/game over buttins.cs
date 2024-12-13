using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameoverbuttins : MonoBehaviour
{
    [SerializeField] private Button restart;
    [SerializeField] private Button mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        restart.onClick.AddListener(() => restartClick());
        mainMenu.onClick.AddListener(() => mainMenuClick());
    }

    private void restartClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dock Thing 1");
    }

    private void mainMenuClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        CharacterSelection.count = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
