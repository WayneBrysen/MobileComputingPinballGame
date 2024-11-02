using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBallControl : MonoBehaviour
{
    public float gravityMultiplier = 9.81f;
    private Vector3 originalGravity;

    void Start()
    {
        Input.gyro.enabled = true;

        originalGravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        if (Input.gyro.enabled)
        {
            Vector3 tilt = Input.gyro.gravity;

            Physics.gravity = new Vector3(tilt.x, -tilt.z, tilt.y) * gravityMultiplier;
        }
    }

    void OnDestroy()
    {
        Physics.gravity = originalGravity;
    }
}