using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleScene_PlayerRun : MonoBehaviour
{
    public float speed = 10.0f;
    private float xLeftBound = -6.6f; // x boundary [-6.6, 5]
    private float xRightBound = 5.0f; // x boundary [-6.6, 5]

    private float zUpperBound = 12.0f;
    private float zLowerBound = 0.0f; // z boundary [0, 12]
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Run();
    }

    void Run()
    {
        float x = transform.position.x;
        float z = transform.position.z;

        if (x <= xLeftBound && z > zLowerBound)
        {
            // Move down
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (z - speed * Time.deltaTime <= zLowerBound)
            {
                // Rotate 90 degrees to the right
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        else if (z >= zUpperBound && x > xLeftBound)
        {
            // Move left
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (x - speed * Time.deltaTime <= xLeftBound)
            {
                // Rotate 90 degrees to the right
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else if (x >= xRightBound && z < zUpperBound)
        {
            // Move up
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (z + speed * Time.deltaTime >= zUpperBound)
            {
                // Rotate 90 degrees to the right
                transform.rotation = Quaternion.Euler(0, 270, 0);
            }
        }
        else if (z <= zLowerBound && x < xRightBound)
        {
            // Move right
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (x + speed * Time.deltaTime >= xRightBound)
            {
                // Rotate 90 degrees to the right
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
