using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{

    /* . . . OTHER OBJECTS . . . */
    [SerializeField] private LevelPicker levelPicker;
    [SerializeField] private GameObject[] speedImgs;
    [SerializeField] private GameObject[] moneyGrabImgs;

    [SerializeField] private TextMeshProUGUI speedTxt;
    [SerializeField] private TextMeshProUGUI moneyGrabTxt;

    private int[] costs = {20, 50, 100}; //costs to upgrade at each level
    private string[] costNames = {"$20", "$50", "$100", "MAX"}; //text array 

    private int _speedLevel, _moneyGrabLevel, _totalCoins; //current speed and moneyGrab update levels, and total coins



    /* . . . TEXT POPUPS . . . */
    private float _txtFadeSpeed = 0.05f; //text fade speed
    [SerializeField] private CanvasRenderer fundTxt; //"Insufficient Funds" text
    [SerializeField] private CanvasRenderer maxUpgradeTxt; //"Already at max upgrade" text
    private int _fundFadeState = 0; //disappeared, fade in, fade out
    private int _maxUpgradeFadeState = 0; //disappeared, fade in, fade out

    

    /* . . . BUTTON AUDIO . . . */
    [SerializeField] private ButtonAudio buttonAudio;



    void Start() {
        
        //update variables depending on previously upgraded levels
        _speedLevel = GameManager.upgradeLevels[0];
        _moneyGrabLevel = GameManager.upgradeLevels[1];
        _totalCoins = GameManager.totalCoins;

        //show images depending on upgraded levels
        speedImgs[_speedLevel].SetActive(true);
        moneyGrabImgs[_moneyGrabLevel].SetActive(true);

        //show cost depending on upgraded levels
        speedTxt.text = costNames[_speedLevel];
        moneyGrabTxt.text = costNames[_moneyGrabLevel];


        //text popups fully transparent
        fundTxt.SetAlpha(0f);
        maxUpgradeTxt.SetAlpha(0f);

    }


    void LateUpdate() {
        
        if(_fundFadeState == 1) { //fade in

            fundTxt.SetAlpha(fundTxt.GetAlpha()+_txtFadeSpeed); //increase opacity
            if(fundTxt.GetAlpha() >= 2.5f) _fundFadeState = 2; //when reached goal opacity, fade out


        } else if (_fundFadeState == 2) { //fade out

            fundTxt.SetAlpha(fundTxt.GetAlpha()-_txtFadeSpeed); //decrease opacity

            if(fundTxt.GetAlpha() < 0) { //when reached goal opacity, reset
                _fundFadeState = 0;
                fundTxt.SetAlpha(0f);
            }

        }


        if(_maxUpgradeFadeState == 1) { //fade in

            maxUpgradeTxt.SetAlpha(maxUpgradeTxt.GetAlpha()+_txtFadeSpeed); //increase opacity
            if(maxUpgradeTxt.GetAlpha() >= 2.5f) _maxUpgradeFadeState = 2; //when reached goal opacity, fade out

        } else if (_maxUpgradeFadeState == 2) { //fade out

            maxUpgradeTxt.SetAlpha(maxUpgradeTxt.GetAlpha()-_txtFadeSpeed); //decrease opacity

            if(maxUpgradeTxt.GetAlpha() < 0) { //when reached goal opacity, reset
                _maxUpgradeFadeState = 0;
                maxUpgradeTxt.SetAlpha(0f);
            }

        }


    }
    

    //for when user clicks to level up speed upgrade
    public void SpeedButton() {

        if(_speedLevel >= 3) _maxUpgradeFadeState = 1; //check if already upgraded to max level
        else if (_totalCoins < costs[_speedLevel]) _fundFadeState = 1; //check if player has enough coins for upgrade


        if (_speedLevel < 3 && _totalCoins >= costs[_speedLevel]) { //if both requirements satisfy (not at max, enough coins)
            
            speedImgs[_speedLevel].SetActive(false); 

            _totalCoins -= costs[_speedLevel]; //decrease total coins (spent on upgrade)
            _speedLevel += 1; //increase upgrade level

            speedImgs[_speedLevel].SetActive(true); //show new image with upgraded level displayed

            
            //update GameManager upgrade levels
            GameManager.upgradeLevels[0] = _speedLevel;
            GameManager.totalCoins = _totalCoins;
            speedTxt.text = costNames[_speedLevel]; //update cost text

            //play click sound
            buttonAudio.ClickSound();

        }
        
        
        //update coins text
        levelPicker.updateCoins();

    }


    //for when user clicks to level up moneyGrab upgrade
    public void MoneyGrabButton() {

        if(_moneyGrabLevel >= 3) _maxUpgradeFadeState = 1; //check if already upgraded to max level
        else if (_totalCoins < costs[_moneyGrabLevel]) _fundFadeState = 1; //check if player has enough coins for upgrade


        if (_moneyGrabLevel < 3 && _totalCoins >= costs[_moneyGrabLevel]) { //if both requirements satisfy (not at max, enough coins)

            moneyGrabImgs[_moneyGrabLevel].SetActive(false);

            _totalCoins -= costs[_moneyGrabLevel]; //decrease total coins (spent on upgrade)
            _moneyGrabLevel += 1; //increase upgrade level

            moneyGrabImgs[_moneyGrabLevel].SetActive(true); //show new image with upgraded level displayed


            //update GameManager upgrade levels
            GameManager.upgradeLevels[1] = _moneyGrabLevel;
            GameManager.totalCoins = _totalCoins;
            moneyGrabTxt.text = costNames[_moneyGrabLevel]; //update cost text


            //play click sound
            buttonAudio.ClickSound();

        } 


        //update coins text
        levelPicker.updateCoins();

    }

    

}
