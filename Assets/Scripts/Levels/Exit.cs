using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{

    /* . . . OTHER OBJECTS . . . */
    [SerializeField] LevelManager levelManager;
    [SerializeField] UserInterface userInterface;
    [SerializeField] GameObject exitObstacle; //exit door
    [SerializeField] GameObject lockSprite; //lock image sprite on top of door when jackpot not collected
    [SerializeField] AudioManager audioManager;

    public bool init = false; //whether or not initialized (if initialized, then player can exit level, player should only be able to exit after jackpot collected)


    public void Init() { //if jackpot collected, then player can exit the house and finish the level

        init = true;
        exitObstacle.transform.Rotate(0, 90, 0); //open exit door
        lockSprite.SetActive(false); //make lock on top of exit door disapear

    }


    void OnTriggerEnter(Collider other) { 

        if(init && other.tag == "Player") { //if player exits the house (collides with game object outside exit door)

            userInterface.Win(); //open win panel
            GameManager.LevelComplete(levelManager.levelInt); //update levelcomplete levels in game manager

            audioManager.LevelEnd(); //initiate level end audio

        }
    }

}
