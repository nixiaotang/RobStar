using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    
    //audio sources for click sound and hover sound
    [SerializeField] private AudioSource _clickSound;
    [SerializeField] private AudioSource _hoverSound;

    /*

        When the player clicks a button to change scenes and button sound is played,
        the button sound may be cut off after scene switches

        So, the game object is not destroyed after scene change
        After sound is finished playing and if scene is different, then self destroy the game object to prevent multiple copies

    */

    //whether or not in the original scene
    private bool sameScene = true;


    //function that runs once when gameObjects first loaded
    void Awake() {
        DontDestroyOnLoad(transform.gameObject); //prevents game object from being destroyed after scene change
    }


    void LateUpdate() {

        //if sounds are finished playing and scene isnt the original scene, destroy itself
        if (!sameScene && !_clickSound.isPlaying && !_hoverSound.isPlaying) { 
            Destroy(this.gameObject);

        }

    }


    //function to play click sound
    public void ClickSound() {
        _clickSound.Play();
    }

    //function to play click sound for scene switching buttons
    public void ClickSoundSwitchScene() {
        _clickSound.Play();
        sameScene = false; //not in original scene anymore
    }

    //function to play hover sound
    public void HoverSound() {
        _hoverSound.Play();
    }
}
