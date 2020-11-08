using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    
    /* . . . OTHER OBJECTS . . . */
    private LevelManager levelManager;
    private Transform player;
    private SphereCollider coinCollider;


    /* . . . COIN VARS . . . */
    private bool _collected = false; //whether or not this coin has been collected
    private float _disappearSpeed = 0.06f; //speed at which coin turns small and disappears
    private float _collectSpeed = 0.2f; //speed of coin moving towards player (simulates collection)

    private float[] _colliderRadii = {1f, 1.5f, 1.9f, 2.25f}; //size of coin collider for different moneyGrab upgrade levels



    void Start() {

        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>(); //gets LevelManager script
        player = GameObject.FindGameObjectWithTag("Player").transform; //gets transform of player

        //select collider component and set radius depending on moneyGrab upgrade level
        coinCollider = GetComponent<SphereCollider>();
        coinCollider.radius = _colliderRadii[GameManager.upgradeLevels[1]];
    }



    void LateUpdate() {

        if(_collected) { //if coin is collected

            Vector3 _goal = new Vector3(player.position.x, this.transform.position.y, player.position.z); //goal pos = player pos
            Vector3 _direction = _goal - this.transform.position; //direction to move to goal (player)

            _direction.Normalize(); //normalize for equal movement speed

            this.transform.position += _direction * _collectSpeed; //move coin towards player with set speed


            this.transform.localScale -= new Vector3(_disappearSpeed, _disappearSpeed, _disappearSpeed); //slowly turn smaller

            //if disappeared, destroy self
            if(this.transform.localScale.x <= 0f) Destroy(this.gameObject);

        }

    }

    
    //if player collides with coin collider
    private void OnTriggerEnter(Collider other) {
        
        //casts a ray towards the player, if the first thing it hits is the player, then coin is collectable
        //prevents the player from collecting coins across walls, or other obstacles
        RaycastHit rayHit;
        Vector3 _goal = new Vector3(player.position.x, this.transform.position.y, player.position.z);
        Vector3 _direction = _goal - this.transform.position; //direction to move to goal (player)

        //shoots a ray from position (coin pos) towards player)
        if(Physics.Raycast(this.transform.position, _direction, out rayHit)) {

            
            //if ray hits the player and coin is colliding with the player, collected is true
            if(rayHit.transform.gameObject.tag == "Player" && other.tag == "Player") {

                _collected = true;
                
                levelManager.addCollected(); //add to the total collectable count
                levelManager.addCoins(10); //add to total coin count (currency)
                
            }

        }

    }
    
}
