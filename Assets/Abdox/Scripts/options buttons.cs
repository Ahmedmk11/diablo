using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class optionsbuttons : MonoBehaviour
{
    [SerializeField] private Button back;
    [SerializeField] private Slider sound;
    [SerializeField] private Slider effect;
    // Start is called before the first frame update
    void Start()
    {
        back.onClick.AddListener(() => backClick());
        sound.onValueChanged.AddListener((float value) => soundClick(value));
        effect.onValueChanged.AddListener((float value) => effectClick(value));
    }

    private void backClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void soundClick(float value)
    {
        PlayerPrefs.SetFloat("sound", value);
    }

    private void effectClick(float value)
    {
        PlayerPrefs.SetFloat("effect", value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
