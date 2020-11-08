using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //background music script
    private MenuMusic menuMusic;

    void Start() {

        //get script component for background music
        menuMusic = GameObject.FindGameObjectWithTag("Music").GetComponent<MenuMusic>();
        menuMusic.PlayMusic(); //play music
    }


    //function to start the game (scene switch to level picker scene)
    public void Play() {
        SceneManager.LoadScene(1);
    }

    //function to quit the game
    public void Quit() {
        Debug.Log("QUIT!");
        Application.Quit();
    }


    void LateUpdate() {
        
        //quit application if escape key pressed
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("QUIT!");
            Application.Quit();
        }
    }
}
