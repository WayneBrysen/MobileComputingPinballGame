using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBallControl : MonoBehaviour
{

    // Start is called before the first frame update
     void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
     void Update()
    {
        if (Input.gyro.enabled)
        {
            Debug.Log(Input.gyro.attitude);
            transform.rotation = Input.gyro.attitude;

        }
    }
}
