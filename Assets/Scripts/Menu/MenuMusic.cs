using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{   
    
    //audio source for background music
    private AudioSource _menuMusic;

    private void Awake() {

        DontDestroyOnLoad(transform.gameObject); //don't destroy object when switch to new scene
        _menuMusic = GetComponent<AudioSource>(); //get audiosource component

        //if this is not the first time script has been loaded, then self destroy to prevent copies of itself
        if(!GameManager.firstLoad) Destroy(this.gameObject);

    }


    void Start () {
        GameManager.firstLoad = false; //indicates that game has been loaded for the first time
    }


    //play background music function
    public void PlayMusic() {
        if (_menuMusic.isPlaying) return; //if already playing music, return
        
        _menuMusic.Play(); //otherwise, play music (start from beginning)
    }
    
    //stop background music function
    public void StopMusic() {
        _menuMusic.Stop(); //stop music
    }

}
