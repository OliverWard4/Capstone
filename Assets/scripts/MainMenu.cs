using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button StartButton;

    public void Start()
    {
         Cursor.visible = true;
        
    }


    public void StartGame(bool hit)
    {
        print("Game Start");
        SceneManager.LoadScene("Level1");
    }

    public void CloseGame()
    {
        Application.Quit(); 
    }


    public void HowToPlay(Image Instructions)
    {
        Instructions.enabled = !Instructions.enabled;
        Instructions.GetComponentInChildren<TextMeshProUGUI>().enabled = Instructions.enabled;

    }
}
