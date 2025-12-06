using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas Menu;
    [SerializeField] private TextMeshProUGUI LevelComplete; 
    [SerializeField] private TextMeshProUGUI LevelLost; 


    [SerializeField] private InteractableManager GameController;
    [SerializeField] private String NextLevel = "MainMenu";
    [SerializeField] private bool OnWinScreen = false; 

    private bool PlayerLost = false; 

    // Start is called before the first frame update
    void Start()
    {
        if (Menu != null && !OnWinScreen) { Menu.gameObject.SetActive(false); }
        if (LevelComplete != null) {LevelComplete.gameObject.SetActive(false); }
        if (LevelLost != null) { LevelLost.gameObject.SetActive(false); }
        PlayerLost = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            print("Key pressed");
            if (Menu.gameObject.activeSelf)
            {
                Menu.gameObject.SetActive(false);
            }
            else
            {
                Menu.gameObject.SetActive(true);
            }
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void Continue()
    {
        if (GameController.LevelComplete())
        {
            SceneManager.LoadScene(NextLevel);
        } else if (PlayerLost)
        {
            Retry(); 
        }
        else
        {
            Menu.gameObject.SetActive(false);
        }   
     }

    public void Quit()
    {
        SceneManager.LoadScene("Main_Menu");

    }

    public void WinScreen()
    {
        LevelComplete.gameObject.SetActive(true);
    }

    public void LoseScreen()
    {
        LevelLost.gameObject.SetActive(true);
        PlayerLost = true; 
    }





}
