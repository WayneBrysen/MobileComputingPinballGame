using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBallControl : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float upSpeed = 5f;
    [SerializeField]
    private float maxSpeed = 20f;

    private Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.gyro.enabled)
        {
            // Get the rotation of the phone based on the gyroscope
            Quaternion deviceRotation = Input.gyro.attitude;

            // Convert the gyroscope rotation to world space
            Vector3 ballMovement = new Vector3(deviceRotation.x, 0, deviceRotation.y);

            rb.AddForce(transform.up * upSpeed, ForceMode.Acceleration);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }
}
