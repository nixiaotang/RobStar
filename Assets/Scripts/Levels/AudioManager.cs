using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private string _audioState = "game"; //"game", "chase", "fading"
    public bool[] chasePoliceStates; //stores whether or not police is chasing player (for each police in level)
    private bool _chase = false; //if any police is chasing player, chase is true and should play chase music

    //audio sources to play and stop music
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource chaseMusic;
    [SerializeField] private AudioSource coinCollect;
    private MenuMusic menuMusic;

    private float _musicFadeSpeed = 0.01f; //speed at which music fades to transition into another music
    
    void Start() {

        //get menu background music component to play later
        menuMusic = GameObject.FindGameObjectWithTag("Music").GetComponent<MenuMusic>();
        menuMusic.StopMusic();

        chaseMusic.Stop(); //stop chase music
        gameMusic.Play(); //start playing game music
    }

    void LateUpdate() {

        _chase = false; //starts being false

        //if any police is chasing player, _chase is true
        for(int i = 0; i < chasePoliceStates.Length; i++) {
            if(chasePoliceStates[i]) {
                _chase = true;
                break;
            }
        }


        if(_audioState == "game" && _chase) { //if playing game audio but player is being chased
            chaseMusic.Play(); //play chase music
            gameMusic.Stop(); //stop game music
            _audioState = "chase"; //update audio state

        } else if (_audioState == "chase" && !_chase) { //if playing chase audio but player is not being chased
            _audioState = "fading"; //update audio state
        }


        //if in fading audio state (chase music fading, replaced with game music)
        if(_audioState == "fading") {
            chaseMusic.volume -= _musicFadeSpeed; //decrease volume of chase music

            if(chaseMusic.volume <= 0f) { //when chase music volume reaches 0
                chaseMusic.Stop(); //stop chase music
                gameMusic.Play(); //start game music
                chaseMusic.volume = 1f; //reset chase music volume back to 1

                _audioState = "game"; //update audio state
            }
        }
        
    }


    //called when player beats or fails the level
    public void LevelEnd() { 
        menuMusic.PlayMusic(); //start playing menu background music
        chaseMusic.Stop(); //stop playing chase music
        gameMusic.Stop(); //stop playing level/game music
    }


    public void CoinCollect() {
        coinCollect.Play(); //play coin collect sound effect
    }
}
