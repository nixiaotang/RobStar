using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{

    [SerializeField] private GameObject info; //info
    [SerializeField] private Image infoPanelImg; //semi transparent black background panel
    [SerializeField] private Image infoImg; //image of controls info

    [SerializeField] private int infoInt;

    private Color infoPanelCol; //colour of background panel image
    private Color infoCol; //colour of controls info image



    void Start() {

        if(!GameManager.infoPopups[infoInt]) info.SetActive(true); //display controls info if first time displaying (infoPopups[0] == false)
        
    }


    void LateUpdate() {

        //if player started to move (infoPopups[0] == true) and game object not disabled
        //start decreasing the alpha of controls info images (fade away)
        if(info.activeSelf && GameManager.infoPopups[infoInt]) { 
            
            infoPanelCol = infoPanelImg.color; //get current colour
            infoPanelCol.a -= 0.02f; //decrease alpha val
            infoPanelImg.color = infoPanelCol; //update colour

            infoCol = infoImg.color; //get current colour
            infoCol.a -= 0.02f; //decrease alpha val
            infoImg.color = infoCol; //update colour

            if(infoPanelCol.a <= 0) info.SetActive(false); //if transparency less than 0, disable info game object

        }
        

        
    }
}
