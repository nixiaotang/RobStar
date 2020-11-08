using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public int levelInt; //keeps track of current level integer
    
    
    /* . . . STATES . . . */
    public bool playerAlive = true;
    public bool paused = false;


    /* . . . COINS / COLLECTABLES . . . */
    [SerializeField] private GameObject coinObj; //coin object
    [SerializeField] private Transform[] coinPos; //array of coin positions in the level
    private GameObject _newCoin; //object that holds the newly instantiated coin object

    [SerializeField] private GameObject jackpotObj; //jackpot object
    [SerializeField] private Transform jackpotPos; //position of jackpot in level
    private GameObject _newJackpot; //holds newly instantiated jackpot object

    [SerializeField] private GameObject coinContainer; //object that will hold all the newly instantiated collectables

    public int totalCoins; //total coins in level
    public int collected = 0; //total collected coins
    public bool jackpot = false; //bool that keeps track of whether or not the jackpot has been collected


    /* . . . KEY . . . */
    [SerializeField] private GameObject lockedDoor; //locked door
    [SerializeField] private GameObject lockSprite; //image sprite of lock on top of locked door
    public bool key; //whether or not level requires a key
    public bool keyCollected = false; //whether or not key is collected


    /* . . . ACHEIVEMENTS . . . */
    public bool seen = false;
    

    /* . . . OTHER OBJECTS . . . */
    [SerializeField] private GameObject exit;
    [SerializeField] private UserInterface userInterface;
    [SerializeField] private AudioManager audioManager;


    void Start() {

        _newJackpot = Instantiate(jackpotObj, jackpotPos.position, Quaternion.identity); //instantiate jackpot at pre-determined position
        _newJackpot.transform.rotation = Quaternion.Euler(0, jackpotPos.rotation.eulerAngles.y, 0);
        _newJackpot.transform.parent = coinContainer.transform; //sets coinContainer as parent (organization purposes)
    

        for(int i = 0; i < coinPos.Length; i++) { //for each coin in coinPos array 
            _newCoin = Instantiate(coinObj, coinPos[i].position, Quaternion.identity); //instantiate new coin for each position in coinPos array
            _newCoin.transform.rotation = Quaternion.Euler(0, coinPos[i].rotation.eulerAngles.y, 0);
            _newCoin.transform.parent = coinContainer.transform; //sets parent of new coin
            
        }

        //communicates to UI script for starting text values
        totalCoins = coinPos.Length;
        userInterface.addCollected(0, totalCoins);
        userInterface.updateCoins(GameManager.totalCoins);

        
    }


    //updates number of collected coins, communicates to UI script to update collected text
    public void addCollected() {
        collected += 1;
        userInterface.addCollected(collected, totalCoins);

    }

    //updates number of coins, communicates to UI script to update coins text
    public void addCoins(int coins) {
        GameManager.totalCoins += coins;
        userInterface.updateCoins(GameManager.totalCoins);
        
        audioManager.CoinCollect(); //play coin collected sound effect
    }


    //function ran when key is collected
    public void KeyCollected() {
        lockedDoor.transform.Rotate(0, -90, 0); //rotate locked door (open locked door)
        lockSprite.SetActive(false); //make lock on top of door disappear
        keyCollected = true; //indicate that key is collected

        audioManager.CoinCollect(); //play coin collected sound effect
    }



}
