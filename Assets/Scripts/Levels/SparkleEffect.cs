using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkleEffect : MonoBehaviour
{

    //sparkle images
    [SerializeField] private GameObject[] images;

    private float[] _rotations = {0f, 0f, 0f}; //rotations of all 3 sparkle images
    private float[] _scales = {0.01f, 0.07f, 0.1f}; //scales (size) of all 3 sparkle images
    
    private float _scaleSpeed = 0.003f; //scale speed
    private float[] _scaleSpeeds = {0f, 0f, 0f}; //scale speed for each sparkle image (pos or neg depending on if it's increasing or decreasing in size)
    private float[] _maxScale = {0.14f, 0.1f, 0.08f}; //max scale that each sparkle image can reach

    private float _rotSpeed = 1f; //rotation speed
    


    // Start is called before the first frame update
    void Start() {

        //for each sparkle, start at random rotation and scale, and set scale speed
        for(int i = 0; i < 3; i++) {
            _rotations[i] = Random.Range(0f, 360f);
            _scales[i] = Random.Range(0f, _maxScale[i]*1000);
            _scales[i] /= 1000f;
            _scaleSpeeds[i] = _scaleSpeed;
        }
        
    }

    // Update is called once per frame
    void Update() {
        
        //for each sparkle
        for(int i = 0; i < 3; i++) {

            _rotations[i] += _rotSpeed; //calculate rotation
            _scales[i] += _scaleSpeeds[i]; //calculate scale

            _rotations[i] %= 360f; //makes sure rotation doesnt go over 360
            

            //if scale goes over maxScale (size) or if scale goes below 0, then invert the scale speed 
            if(_scales[i] >= _maxScale[i] || _scales[i] <= 0f) _scaleSpeeds[i] = -_scaleSpeeds[i];


            //scale and rotate the image
            images[i].transform.localScale = new Vector3(_scales[i], _scales[i], _scales[i]);
            images[i].transform.Rotate(0, _rotSpeed, 0);


        }

    }
}
