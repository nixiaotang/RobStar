using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintWalkNavMesh : MonoBehaviour
{

    /* . . . PLAYER & OTHERS. . . */
    [SerializeField] private Transform player;
    [SerializeField] private GameObject lastPlayerPos;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private UserInterface userInterface;
    [SerializeField] private Player playerScript;
 
    UnityEngine.AI.NavMeshAgent agent; //NavMesh agent
    private Animator anim; //animator
    

    /* . . . POLICE STATE . . . */
    [SerializeField] private int policeInt;
    [SerializeField] private string _state = "PATROL";


    /* . . . MOVEMENT . . . */
    private float _walkSpeed = 1.1f, _runSpeed = 3.6f;
    private float _stamina = 5f; //seconds before switching between run and walk
    private float _rotSpeed = 1.5f; //rotation speed
    private float _closeEnough = 0.5f; //close enough to target
    

    /* . . . VISION & LOOK AROUND . . . */
    private float _maxVisionLength = 8.5f; //max length of line of sight 
    private float _maxVisionAngle = 55f; //max angle range of line of vision

    Quaternion _rot1, _rot2; //look around rotations
    private float _curRot; //keeps track of  current police rotation angle (y axis)
    private bool _lookAroundState = true; //controls direction to turn when looking around


     /* . . . COROUTINES . . . */
    IEnumerator chase;
    IEnumerator lookAround;


    /* . . . PATROLS AND WAYPOINTS . . . */
    [SerializeField] private Transform[] waypoints; //patrol waypoint coords
    [SerializeField] private int[] points; //waypoints to pause
    [SerializeField] private GameObject WPDot, dotContainer; //gameObj to be instanciated, parent obj to store WPDots
    private GameObject _newDot; //newly instantiated WPdot

    private int _curWP, _WPLen; //current waypoint index, length of waypoint list
    [SerializeField] private bool _cycle; //whether or not movement pattern is in a cycle
    private bool _forward = true; //direction of travel
    private float _patrolStartTime, _patrolWaitTime = 2f; //controls police patrol waiting
    private bool _waiting = false; //waiting bool


    /* . . . HEARING . . . */
    private float _playerPoliceDist; //keeps track of the distance between the player and police
    private float _sprintHearRadius = 2f; //radius in which police can hear the player if player is running
    private float _walkHearRadius = 1.2f;  //radius in which police can hear the player if player is walking
    private Vector3 _hearPlayerPos; //position where police last heard player
    private Quaternion _hearPlayerRot; //rotation at which police needs to turn to face the position where police last heard player


    /* . . . MUSIC AND SOUNDS . . . */
    [SerializeField] private AudioManager audioManager;


    void Start() {
        
        //get and assign components
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        anim.speed = 1.2f; //animation speed
        
        lastPlayerPos.SetActive(false);
        
        //start walking, patrolling
        _state = "PATROL";
        agent.speed = _walkSpeed;
        

        //WAYPOINTS
        _WPLen = waypoints.Length;
        _curWP = 0;

        //instantiate dotted lines to mark police movement patterns
        for(int j = 0; j < _WPLen-1; j++) { //for each waypoint

            Vector3 _fullLen = waypoints[j+1].position - waypoints[j].position; //diff between cur waypoint and next waypoint
            Vector3 _step = _fullLen; 
            _step.Normalize(); //distance normalized for equal spacing

            //instantiate dot for the full length of the two waypoints
            for (int i = 0; i < Mathf.Round(_fullLen.magnitude / _step.magnitude); i++) { 
                _newDot = Instantiate(WPDot, waypoints[j].position+_step*i, Quaternion.identity);
                _newDot.transform.parent = dotContainer.transform;
            }

        }


    }

    
    void LateUpdate() {

        //if paused, go to idle animation and break out of update loop
        if (levelManager.paused) {
            agent.speed = 0; //stop moving
            anim.SetInteger("state", 0); //idle state
            return;
        }

        
        if(canSeePlayer()) { //if player / robber detected, start chasing

            if(_state != "CHASEPLAYER" && _state != "CHASEPOISITION") {
                _state = "CHASEPLAYER"; // update state
                chase = Chase();
                StartCoroutine(chase); //start chasing coroutine (keeps track of stamina)
                
            }

            //updates last seen player position
            lastPlayerPos.transform.position = new Vector3(player.position.x, lastPlayerPos.transform.position.y, player.position.z);
            lastPlayerPos.SetActive(true);

            levelManager.seen = true; //communicates with level manager

        }


        switch(_state) {
            
            case "PATROL" :

                if(_WPLen > 0) {
                    
                    //goal -> position of curernt waypoint
                    Vector3 _goal = new Vector3(waypoints[_curWP].position.x, this.transform.position.y, waypoints[_curWP].position.z);
                    Vector3 _direction = _goal - this.transform.position; //direction of movement (goal pos - current pos)

                    if(_waiting) anim.SetInteger("state", 0); //idle animation
                    else anim.SetInteger("state", 1); //walking animation

                    if(_direction.magnitude < _closeEnough) { //if close enough to current waypoint

                        //if current waypoint is in points array, then wait for a while before continuing on in the waypoint movement patterns
                        for(int i = 0; i < points.Length; i++) { //loop though each item in the points array

                            if(_curWP == points[i]) { //if current wp is in points array
                                
                                if(!_waiting) { //if not waiting, mark start wait time (patrolStartTime)
                                    _patrolStartTime = Time.time;
                                    _waiting = true; //set waiting to true

                                } else { //if waiting

                                    //set waiting to false if the diff between current time and start wait time is greater than total wait time
                                    if(Time.time - _patrolStartTime >= _patrolWaitTime) _waiting = false;
                                        
                                }

                            }
                        }

                        
                        if(!_waiting) { //if not waiting

                            if(_cycle) { //if cycle mode

                                //increase wp index, go back to first WP if reacehd the end
                                _curWP ++;
                                _curWP = _curWP % _WPLen;

                            } else { //if not cycle mode
                                
                                if(_forward) { //if going forward
                                    if(_curWP == _WPLen-1) _forward = false; //go backward if reached end of WP list
                                    else _curWP ++; //otherwise increase WP index

                                } else { //if going backward
                                    if(_curWP == 0) _forward = true; //go forward if reached end of WP list
                                    else _curWP --; //otherwise decrease WP index
                                }

                            }
                        }
                        
                    }

                    agent.SetDestination(_goal); //agent movement

                }
                
                //calcuates distance between police and player
                _playerPoliceDist = Vector3.Distance(this.transform.position, new Vector3(player.position.x, this.transform.position.y, player.position.z)); 
                
                //if player is running and within _sprintHearRadius or if player is not running but within _walkHearRadius 
                //then police hears player, turns towards where he heard the player and starts looking around
                if ((_playerPoliceDist < _sprintHearRadius && playerScript.run) || _playerPoliceDist < _walkHearRadius) { 

                    _hearPlayerPos = new Vector3(player.position.x, this.transform.position.y, player.position.z); //stores where police last heard player

                    lastPlayerPos.transform.position = new Vector3(_hearPlayerPos.x, lastPlayerPos.transform.position.y, _hearPlayerPos.z); //update red spot
                    lastPlayerPos.SetActive(true); //set red spot to be visible

                    agent.speed = 0; //stop moving
                    anim.SetInteger("state", 0); //idle state

                    _state = "HEARSOUND"; //switch states
                
                }

            
                break;
            
            case "HEARSOUND" :

                _hearPlayerRot = Quaternion.LookRotation(_hearPlayerPos - this.transform.position); //rotation to turn the police to face towards where police last heard player
                transform.rotation = Quaternion.Slerp(this.transform.rotation, _hearPlayerRot, _rotSpeed * Time.deltaTime); //gradually turn

                if(Quaternion.Angle(this.transform.rotation, _hearPlayerRot) < 3f) { //if police rotation close to goal rotation, initiate LOOKAROUND state

                    lastPlayerPos.SetActive(false);

                    //determine rotations for look around scan
                    _curRot = this.transform.rotation.eulerAngles.y; //current rotation
                    _rot1 = Quaternion.Euler(0, _curRot + 90, 0); //first rotation to scan to
                    _rot2 = Quaternion.Euler(0, _curRot - 90, 0); //second rotation to scan to
                    _lookAroundState = true; //state to keep track of scanning to rot1 or rot2


                    _state = "LOOKAROUND";

                }

            break;
            case "CHASEPLAYER" :

                audioManager.chasePoliceStates[policeInt] = true; //indicate to audioManager to play chase sounds

                //update lastPlayerPos variable to player position
                lastPlayerPos.transform.position = new Vector3(player.position.x, lastPlayerPos.transform.position.y, player.position.z);

                if(canSeePlayer()) { //if player detected, chase player

                    agent.SetDestination(player.position);

                } else { //if player not seen, change state to chase position (last player seen position)

                    _state = "CHASEPOSITION";

                }

                break;
            

            case "CHASEPOSITION" :

                audioManager.chasePoliceStates[policeInt] = true; //indicate to audioManager to play chase sounds

                agent.SetDestination(lastPlayerPos.transform.position); //chase lastPlayerPos (last seen player position)

                if(Vector3.Distance(this.transform.position, lastPlayerPos.transform.position) < _closeEnough) { 
                    StopCoroutine(chase); //stop chasing coroutine

                    _state = "LOOKAROUND"; //change state
                    lastPlayerPos.SetActive(false);

                    //determine rotations for look around scan
                    _curRot = this.transform.rotation.eulerAngles.y; //current rotation
                    _rot1 = Quaternion.Euler(0, _curRot + 90, 0); //first rotation to scan to
                    _rot2 = Quaternion.Euler(0, _curRot - 90, 0); //second rotation to scan to
                    _lookAroundState = true; //state to keep track of scanning to rot1 or rot2

                }

                break;
            
            case "LOOKAROUND" :

                audioManager.chasePoliceStates[policeInt] = false; //indicate to audioManager that this specific police is not chasing player

                agent.speed = 0; //stop moving
                anim.SetInteger("state", 0); //idle state

                if(_lookAroundState) { //gradually turn right

                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, _rot1, _rotSpeed * Time.deltaTime);
                    if(Quaternion.Angle(this.transform.rotation, _rot1) < 3f) _lookAroundState = false;

                } else { //gradually turn left

                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, _rot2, _rotSpeed * Time.deltaTime);

                    if(Quaternion.Angle(this.transform.rotation, _rot2) < 3f) { //return to patrol state
                        _lookAroundState = true;
                        _state = "PATROL";

                        //walking speed and animation
                        agent.speed = _walkSpeed;
                        anim.SetInteger("state", 1);
                    }

                }

                break;


        }
    }
    
    

    //police vision
    bool canSeePlayer() {

        Vector3 toTarget = player.position - this.transform.position; //vector towards target (player)
        float lookAngle = Vector3.Angle(this.transform.forward, toTarget); //angle between forward and target

        RaycastHit rayHit; //raycasting


        //draw ray to visualize rays (debugging and visualizing purposes, not shown in game)
        Debug.DrawRay(this.transform.position + Vector3.up*1.7f, this.transform.forward*_maxVisionLength, Color.green);
        Debug.DrawRay(this.transform.position + Vector3.up*1.7f, Quaternion.Euler(0, _maxVisionAngle, 0) * this.transform.forward *_maxVisionLength, Color.red);
        Debug.DrawRay(this.transform.position + Vector3.up*1.7f, Quaternion.Euler(0, -_maxVisionAngle, 0) * this.transform.forward *_maxVisionLength, Color.red);
        

        //shoot ray at player 
        if(Physics.Raycast(this.transform.position + Vector3.up*1.7f, toTarget, out rayHit)) {

            //if ray hit is player (nothing blocking police and player),
            //  player is within vision angle,
            //  and player is within vision range
            if(rayHit.transform.gameObject.tag == "Player" && 
                lookAngle <= _maxVisionAngle &&
                Vector3.Distance(this.transform.position, player.position) <= _maxVisionLength)  return true; //player seen

        }

        return false; //player not seen

    }

    //chasing (position and player), with stamina
    IEnumerator Chase() {

        while(true) {
            
            //set running speed and animation
            agent.speed = _runSpeed;
            anim.SetInteger("state", 2);

            if(_state != "CHASEPLAYER" && _state != "CHASEPOSITION") yield break; //if not in chase states anymore, break out

            //repeat for a while
            yield return new WaitForSeconds(_stamina);

            //set walking speed and animation
            agent.speed = _walkSpeed;
            anim.SetInteger("state", 1);

            if(_state != "CHASEPLAYER" && _state != "CHASEPOSITION") yield break; //if not in chase states anymore, break out

            //repeat for a while
            yield return new WaitForSeconds(_stamina);



        }

    }



    private void OnTriggerEnter(Collider other) {

        if(userInterface.UIstate != "GAMEOVER" && other.tag == "Player") { //if police collides with player

            userInterface.GameOver(); //open game over panel
            audioManager.LevelEnd(); //initiate level end audio

        }

    }



}


