using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{

    public string UIstate = "GAME"; //GAME, GAMEOVER, WIN
    private int animState = 0; //variable to keep track of animation orders (panel pop ups)


    /* . . . OTHER OBJECTS . . . */
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Text coinTxt; //currency text (used for shop)
    [SerializeField] private Text collectedTxt; //collected num (how many collectables collected out of total in specific level)
    [SerializeField] private GameObject gameOver; //gameOver panel
    [SerializeField] private GameObject levelComplete; //levelComplete panel


    /* . . . IMAGE ASSETS . . . */

    //award image assets (police, money in colour and B/W)
    [SerializeField] private GameObject policeBW; 
    [SerializeField] private GameObject moneyBW;
    [SerializeField] private GameObject policeColour;
    [SerializeField] private GameObject moneyColour;

    [SerializeField] private GameObject[] images; //levelComplete panel array to keep track of asset images
    [SerializeField] private GameObject[] images2; //gameOver panel array to keep track of asset images



    /* . . . ANIMATION . . . */

    //levelComplete panel animation variables
    private float[] _animSpeeds = {0.06f, 0.8f, 0.08f, 0.1f, 0.1f, 0.1f}; //speed at which asset expands out
    private float[] _maxSize = {1.1f, 1f, 1.1f, 1f, 1f, 1.1f}; //max size that asset should be

    //gameOver panel animation variables
    private float[] _animSpeeds2 = {0.06f, 0.8f, 0.1f}; //speed at which asset expands out
    private float[] _maxSize2 = {1.1f, 1f, 1.1f}; //max size that asset should be

    //key sprite appear in top bar
    [SerializeField] private GameObject key;
    private float _keySpeed = 0.05f;
    private float _maxKeySize = 0.55f;




    void Start() {

        if(levelManager.key) key.transform.localScale = new Vector3(0, 0, 0); //set scale of coloured key image sprite to 0 (invisible)
            
    }


    void LateUpdate() {

        //quit application if escape key pressed
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("QUIT!");
            Application.Quit();
        }
        
        //if key collected, make coloured key pop up in top bar
        if(levelManager.key && levelManager.keyCollected && key.transform.localScale.x < _maxKeySize) {
            key.transform.localScale += new Vector3(_keySpeed, _keySpeed, _keySpeed); 

        }


        switch(UIstate) {

            case "GAMEOVER" : //if gameOver

                if (animState < 3) { //if animation not over

                    //expand the current image asset (depending on animState) with the corresponding animation speed (also depedning on animState)
                    images2[animState].transform.localScale += new Vector3(_animSpeeds2[animState], _animSpeeds2[animState], _animSpeeds2[animState]);

                    //if size of image asset reached desired size (maxSize), then stop expanding, move on to next animation (animState++)
                    if(images2[animState].transform.localScale.x >= _maxSize2[animState]) {
                        images2[animState].transform.localScale = new Vector3(_maxSize2[animState], _maxSize2[animState], _maxSize2[animState]);
                        animState ++;

                    }

                }

            break;
            case "WIN" : //if win

                if (animState < 6) { //if animation not over

                    //expand the current image asset (depending on animState) with the corresponding animation speed (also depedning on animState)
                    images[animState].transform.localScale += new Vector3(_animSpeeds[animState], _animSpeeds[animState], _animSpeeds[animState]);

                    //if size of image asset reached desired size (maxSize), then stop expanding, move on to next animation (animState++)
                    if(images[animState].transform.localScale.x >= _maxSize[animState]) {
                        images[animState].transform.localScale = new Vector3(_maxSize[animState], _maxSize[animState], _maxSize[animState]);
                        animState ++;

                    }

                }


            break;
        }




    }


    //update collected text
    public void addCollected(int collected, int total) {
        collectedTxt.text = collected.ToString() + " / " + total.ToString();
    }

    //update coin text
    public void updateCoins(int coins) {
        coinTxt.text = coins.ToString();
    }


    public void Win() {

        UIstate = "WIN"; //set UI state
        levelManager.paused = true; //pause the game (keyboard controlls for player movement doesn't work)

        if(levelManager.collected == levelManager.totalCoins) images[3] = moneyColour; //if all coins collected, use coloured money image
        else images[3] = moneyBW; //otherwise use BW money image (not fulfilled)

        if(levelManager.seen) images[4] = policeBW; //if player seen during level, use BW police image (not fulfilled)
        else images[4] = policeColour; //otherwise use coloured police image


        //set scale of all image assets to 0 (can't be seen)
        for(int i = 0; i < 6; i++) {
            images[i].transform.localScale = new Vector3(0, 0, 0);
        }

        //set panels and images to be active (to appear)
        levelComplete.SetActive(true);
        images[3].SetActive(true);
        images[4].SetActive(true);

    }


    public void GameOver() {
        UIstate = "GAMEOVER"; //set UI state
        levelManager.paused = true; //pause the game (keyboard controlls for player movement doesn't work)

        //set scale of all image assets to 0 (can't be seen)
        for(int i = 0; i < 3; i++) {
            images2[i].transform.localScale = new Vector3(0, 0, 0);
        }

        //set panels to be active (to appear)
        gameOver.SetActive(true);

    }


    //replay button, reload current scene
    public void Replay() {
        SceneManager.LoadScene(levelManager.levelInt+1);
    }

    //go to menu scene
    public void Menu() {
        SceneManager.LoadScene(0);
    }

    //go to level picker scene
    public void LevelPicker() {
        SceneManager.LoadScene(1);
    }

    //go to level picker scene and open shop
    public void Shop() {
        SceneManager.LoadScene(1);
        GameManager.shopState = 1;

    }

}
