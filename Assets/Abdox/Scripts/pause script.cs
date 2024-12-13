using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class pausescript : MonoBehaviour
{
    [SerializeField] private Button resume;
    [SerializeField] private Button restart;
    [SerializeField] private Button mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        resume.onClick.AddListener(() => resumeClick());
        restart.onClick.AddListener(() => restartClick());
        mainMenu.onClick.AddListener(() => mainMenuClick());
    }

    private void resumeClick()
    {
        GetComponent<yarab>().pause();
    }

    private void restartClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;

    }

    private void mainMenuClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        CharacterSelection.count = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
