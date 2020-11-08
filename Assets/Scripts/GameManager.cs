using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static int levelsCompleted = 0; //keeps track of total levels completed 
    public static int totalCoins = 0; //keeps track of total coins (currency for shop)

    public static int[] upgradeLevels = {0, 0}; //upgrade levels [player speed, grabbing]

    public static float[] upgradeSpeeds = {1f, 1.2f, 1.4f, 1.6f}; //player speeds at different upgrade levels

    public static int shopState = 0; //state of shop (open, close)

    public static bool[] infoPopups = {false, false}; //keyboard and mouse controls

    public static bool firstLoad = true; //varaible to make sure certain things only load once


    public static void LevelComplete(int level) {
        levelsCompleted = Mathf.Max(level, levelsCompleted); //update levelsCompleted var if a new level complete
        
    }

}
