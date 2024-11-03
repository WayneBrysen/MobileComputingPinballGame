using UnityEngine;
using UnityEngine.UI;

public class GyroscopeBackgroundColor : MonoBehaviour
{
    private Gyroscope gyro;
    public Image backgroundPanel; 

    private Quaternion initialGyroRotation;

    // Smoothing speed for color transition
    public float colorTransitionSpeed = 2.0f;

    // Current color of the background
    private Color currentColor = Color.black;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            // Record the initial orientation to use as a reference
            initialGyroRotation = gyro.attitude;
        }
        else
        {
            Debug.LogWarning("Device does not support gyroscope.");
        }
    }

    void Update()
    {
        if (gyro != null && backgroundPanel != null)
        {
            // Get the current orientation
            Quaternion currentGyroRotation = gyro.attitude;

            // Adjust for coordinate system differences
            Quaternion adjustedRotation = GyroToUnity(currentGyroRotation);

            // Calculate the difference from the initial orientation
            Quaternion deltaRotation = adjustedRotation * Quaternion.Inverse(GyroToUnity(initialGyroRotation));

            // Convert quaternion to Euler angles
            Vector3 eulerAngles = deltaRotation.eulerAngles;

            // Normalize angles to a 0-1 range for RGB components
            float red = Mathf.Repeat(eulerAngles.x / 360.0f, 1.0f);
            float green = Mathf.Repeat(eulerAngles.y / 360.0f, 1.0f);
            float blue = Mathf.Repeat(eulerAngles.z / 360.0f, 1.0f);

            // Calculate the target color based on the current orientation
            Color targetColor = new Color(red, green, blue);

            // Smoothly interpolate between the current color and the target color
            currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);

            // Set the background color
            backgroundPanel.color = currentColor;
        }
    }

    // Adjust the gyroscope attitude to Unity's coordinate system
    private Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
