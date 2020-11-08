using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{

    public Slider slider;
    public float stamina = 1f; //keeps track of stamina amount (out of 1)


    void LateUpdate() {
        slider.value = stamina; //update slider depending on how much staminal left
        
    }
}
