using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOD : MonoBehaviour
{  


    /*

        THIS SCRIPT IS USED FOR TESTING AND DEBUGGING PURPOSES ONLY
        IT FORCE COMPLETES A LEVEL
        NOT ENABLED IN FINAL GAME

    */


    //other objects
    [SerializeField] private UserInterface userInterface;
    [SerializeField] private LevelManager levelManager;
    
    // Update is called once per frame
    void Update() {

        if(Input.GetKeyDown(KeyCode.G)) { //if G key is pressed, force win the level

            userInterface.Win(); //open win panel
            GameManager.LevelComplete(levelManager.levelInt); //update completed levels in GameManager

        }
        
    }
}
