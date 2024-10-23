using UnityEngine;
using System;

public class Screenshot : MonoBehaviour
{
    public void CaptureScreenshot()
    {
        // Take a screenshot with a timestamped filename
        string fileName = "screenshot-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
        ScreenCapture.CaptureScreenshot(fileName, 4);
        Debug.Log("Screenshot saved as: " + fileName);
    }
}
