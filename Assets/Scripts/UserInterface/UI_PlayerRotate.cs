using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerRotate : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        
    }

    void RotatePlayer()
    {
        //y축에 대해 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);


    }
}
