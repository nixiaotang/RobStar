using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    /* . . . LEVEL . . . */
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Stamina stamina;
    private Camera camera;



    /* . . . ANIMATION . . . */
    private Animator _anim;



    /* . . . MOVEMENT . . . */
    UnityEngine.AI.NavMeshAgent agent;
    private float _walkSpeed, _runSpeed, _speed;
    private Vector3 move;

    //STAMINA
    [SerializeField] private float _stamina = 0f; //amount of stamina 
    public bool run = false; //whether or not player is running
    private bool _SpaceKey = false; //whether or not space key is pressed
    private float _startStamina, _startTime; //amount of stamina when player presses SPACE, time at which player presses SPACE

    //ROTATION
    private float _rotSpeed = 3f;
    private Ray _mouseRay; //ray that shoots from mouse position to 3D space game environment
    private RaycastHit _hit; //info about ray hit
    private Vector3 _mousePos; //mouse position in 3D space
    private Quaternion _lookRot; //rotation that player has to turn to face the mouse position



    void Start() {

        //get camera script component and assign
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //walk and run speeds depend on upgraded speed level
        _walkSpeed = GameManager.upgradeSpeeds[GameManager.upgradeLevels[0]]; 
        _runSpeed = 3*GameManager.upgradeSpeeds[GameManager.upgradeLevels[0]];

        _anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        _anim.speed = _walkSpeed; //animation speed depends on upgraded speed level

        _speed = _walkSpeed; //start off walking

    }

    void LateUpdate() {

        //freeze player if game paused, exit update loop
        if (levelManager.paused) {
            _anim.SetBool("Idle", true);
            run = false;
            return;
        }


        //player movement and rotation
        Move();
        Rotate();

        stamina.stamina = 1 - (_stamina / 3f); //communicates to stamina script on the amount of stamina left
        
    }


    void Move() {

        //get keyboard input
        float VInput = Input.GetAxis("Vertical");


        if(VInput == 0) { // if not moving, idle animation, increase stamina count
            _anim.SetBool("Idle", true);
            _stamina = Mathf.Max(0, _stamina - 0.02f);

        } else {

            if (camera.zoomState == 2) camera.zoomState = 3; //if moving and camera zoomed out, then zoom back in (so player can't move around in zoomed out view)

            if(levelManager.levelInt == 1 || levelManager.levelInt == 2) {
                GameManager.infoPopups[levelManager.levelInt-1] = true; //if moving, close info popup at beginning of lvl1 or lvl2
            }

            _anim.SetBool("Idle", false);

            if(Input.GetKeyDown(KeyCode.Space)) { //first pressed down SPACE, initalize stamina vars
                _SpaceKey = true;
                _startTime = Time.time; //time at which player starts running
                _startStamina = _stamina;

            } else if (Input.GetKeyUp(KeyCode.Space)) { //first released SPACE, stop running
                _SpaceKey = false;
                run = false;
            }

            if(_SpaceKey) { //if SPACE pressed down, calculate stamina increase and deterine if player can run

                _stamina = _startStamina + Time.time-_startTime; //when running, add stamina (the more stamina, the less the player can run, it's inverted and a tiny bit confusing heh)
                _stamina = Mathf.Min(_stamina, 3f); //makes sure stamina doesn't go over limit

                if(_stamina >= 3f) run = false; //can't run if stamina goes over limit
                else run = true;

            }

            if(run) { //if player is running, set running animation var to true, change speed to run speed
                _anim.SetBool("Run", true);
                _speed = _runSpeed;

            } else { //if player is not running, set running animation var to false, change speed to walk spped, increase stamina
                _anim.SetBool("Run", false);
                _speed = _walkSpeed;

                _stamina = Mathf.Max(0, _stamina - 0.02f); //stamina regenerate
            }
            

        }

        //movement
        move = new Vector3(0, 0, VInput * _speed * Time.deltaTime);
        move = this.transform.TransformDirection(move);
        agent.Move(move); //move player


    }


    void Rotate () {
        
        _mouseRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition); //origin of ray at mouse position

        if (Physics.Raycast(_mouseRay, out _hit)) { //cast a ray into 3D space
            _mousePos = new Vector3(_hit.point.x, 0, _hit.point.z); //set mousePos depending on ray hit position
        }

        _lookRot = Quaternion.LookRotation(_mousePos - this.transform.position); //calculate rotation from current player position to mouse position
        transform.rotation = Quaternion.Slerp(this.transform.rotation, _lookRot, _rotSpeed * Time.deltaTime); //turn towards goal rotation to face where the mouse is
    }




}
