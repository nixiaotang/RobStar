using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPicker : MonoBehaviour
{

    /* . . . OTHER OBJECTS . . . */
    [SerializeField] private Text coinTxt;

    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject openShopButton;
    [SerializeField] private GameObject closeShopButton;
    private float _shopScaleSpeed = 0.08f;

    [SerializeField] private GameObject lvl2Button;
    [SerializeField] private GameObject lvl3Button;


    private MenuMusic menuMusic; //background music player
    

    void Start() {

        //get menu music component and play background music
        menuMusic = GameObject.FindGameObjectWithTag("Music").GetComponent<MenuMusic>();
        menuMusic.PlayMusic();
        

        coinTxt.text = GameManager.totalCoins.ToString(); //update coins text 


        if(GameManager.levelsCompleted >= 2) { //if first two levels completed, then player can access and play any level
            lvl2Button.SetActive(true);
            lvl3Button.SetActive(true);

        } else if(GameManager.levelsCompleted >= 1) { //if only first level completed, player can only access and play the first two levels
            lvl2Button.SetActive(true);
            lvl3Button.SetActive(false);

        } else { //if no levels completed, player can only attempt the first level
            lvl2Button.SetActive(false);
            lvl3Button.SetActive(false);

        }

        shop.transform.localScale = new Vector3(0, 0, 0); //shop disappaer


    }


    void LateUpdate() {

        //quit application if escape key pressed
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("QUIT!");
            Application.Quit();
        }

        if(GameManager.shopState == 1) { //shop panel opening
            
            shop.transform.localScale += new Vector3(_shopScaleSpeed, _shopScaleSpeed, _shopScaleSpeed); //zooming out (increasing shop scale)
            
            if(shop.transform.localScale.x >= 1f) { //if shop size reaches full size (1, 1, 1), stop increasing size
                shop.transform.localScale = new Vector3(1, 1, 1);
                GameManager.shopState = 2; //change shop state

                //disable open shop button and enable close shop button
                openShopButton.SetActive(false); 
                closeShopButton.SetActive(true);
            }

        } else if (GameManager.shopState == 3) {

            shop.transform.localScale -= new Vector3(_shopScaleSpeed, _shopScaleSpeed, _shopScaleSpeed); //decrease shop size
            
            if(shop.transform.localScale.x <= 0f) { //if shop size reaches (0, 0, 0), stop decreasing size
                shop.transform.localScale = new Vector3(0, 0, 0); 
                GameManager.shopState = 0; //change shop state

                //enable open shop button and disable close shop button
                openShopButton.SetActive(true);
                closeShopButton.SetActive(false);
            }


        }



    }

    public void updateCoins() {
        coinTxt.text = GameManager.totalCoins.ToString(); //update coins text 
    }

    //button to switch to menu scene
    public void Menu() {
        SceneManager.LoadScene(0);

    }

    //button to switch to level1 scene
    public void Level1() {
        SceneManager.LoadScene(2);
    }

    //button to switch to level2 scene
    public void Level2() {
        SceneManager.LoadScene(3);

    }

    //button to switch to level3 scene
    public void Level3() {
        SceneManager.LoadScene(4);

    }

    //button to open shop panel
    public void openShop() {
        GameManager.shopState = 1;

    }

    //button to close shop panel
    public void closeShop() {
        GameManager.shopState = 3;
    }


}
