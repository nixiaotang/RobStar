using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    [SerializeField] private Transform player; //position of player
    [SerializeField] private Transform zoomOut; //camera position of zoomed out (fully show entire layout of house)

    private float _x, _z, _px, _pz; //camera x pos, z pos, previousX pos, previousZ pos
    public int zoomState = 0; //0 - zoomed in, 1 - zooming out, 2 - zoomed out, 3 - zooming in
    private float _zoomSpeed = 1f; //speed that camera zooms in and out


    /* . . . SPRINT SOUND . . . */
    private bool _prevFrameRun = false; //to store whether or not the player was running in the previous frame
    [SerializeField] private Player playerScript; //player script (to know when player is running)
    [SerializeField] private GameObject sprintSoundObj; //sprint sound game object
    private SpriteRenderer sprintSound; //sprint sound sprite renderer
    private float _fadeSpeed = 0.17f; //speed at which animation fades
    private bool _fade = true; //fading or appearing boolean

    private AudioSource _runningSound; //to play player running sound
    



    void Start() {

        //camera start at player position
        this.transform.position = new Vector3(player.position.x, 11, player.position.z+1);
        
        //reset position variables
        _x = player.position.x;
        _z = player.position.z;
        _px = _x;
        _pz = _z;


        sprintSound = sprintSoundObj.GetComponent<SpriteRenderer>(); //assign sprite renderer component
        sprintSoundObj.SetActive(false); //start of invisible

        _runningSound = GetComponent<AudioSource>(); //assign audio source component

        _runningSound.Stop();

    }


   

    void LateUpdate() {


        //CAMERA MOVEMENT
        //update new position variables
        _x = player.position.x;
        _z = player.position.z;

        //move towards updated player position
        this.transform.Translate(new Vector3(_x-_px, _z-_pz, 0));

        //updates previous x and z positions
        _px = _x;
        _pz = _z;



        //ZOOM IN AND OUT
        //if Q key pressed, zoom in and out depending on current state
        if(Input.GetKeyDown(KeyCode.Q)) {

            if(zoomState == 0) zoomState = 1; //if currently zoomed in, start zooming out
            else if (zoomState == 2) zoomState = 3; //if currently zoomed out, start zooming in

        }

        //call Zoom function to zoom in or out depending on current state
        if(zoomState == 1) Zoom(zoomOut.position, 2);
        else if (zoomState == 3) Zoom(new Vector3(player.position.x, 11, player.position.z+1), 0);




        //RUNNING SPRITE
        //update position of sprint sound sprite 
        sprintSoundObj.transform.position = new Vector3(player.position.x, 0.3f, player.position.z);

        //if player is running, make sprint sound sprite visible and start animating
        if(playerScript.run) {

            sprintSoundObj.SetActive(true); //make sprite appear

            if(_fade) {
                sprintSound.color = new Color(1f, 1f, 1f, sprintSound.color.a - _fadeSpeed); //decrease opacity
                if(sprintSound.color.a < 0f) _fade = false; //if at lowest opacity, start increasing opacity

            } else {
                sprintSound.color = new Color(1f, 1f, 1f, sprintSound.color.a + _fadeSpeed); //increase opacity
                if(sprintSound.color.a >= 1f) _fade = true; //if at top opacity, start fading

            }


        } else {
            sprintSoundObj.SetActive(false); //make sprite dissapear
        }



        //RUNNING AUDIO
        //if player was not running in the previous frame but running in current frame
        //that means player just started running, so play running sound
        if(!_prevFrameRun && playerScript.run)  _runningSound.Play();
        else if (_prevFrameRun && !playerScript.run)  _runningSound.Stop(); //stop running audio in the first frame that player stops running

        //update prevFrameRun state
        _prevFrameRun = playerScript.run;

    }
    


    //function for zooming in and out
    public void Zoom(Vector3 _goal, int newInt) {

        Vector3 _direction = _goal - this.transform.position; //direction to move to goal (camera position)

        _direction /= 7; //smooth movement (slows when closer to goal)

        this.transform.position += _direction * _zoomSpeed; //move camera towards goal

        //if close enough to goal, switch states
        if(Vector3.Distance(this.transform.position, _goal) <= 0.5f) zoomState = newInt;

    }
}
