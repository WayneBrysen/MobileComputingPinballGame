using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FlipperTouchController : MonoBehaviour
{
    // 引用左、右 Flipper 控制器
    public LeftFlipperControl leftFlipper;
    public RightFlipperControl rightFlipper;

    void Update()
    {
        // 检测是否有触摸输入
        if (Input.touchCount > 0)
        {
            // 遍历所有触摸
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                // 获取触摸压力，压力范围通常为0.0到1.0，某些设备可以支持更高的压力值
                float pressure = Mathf.Clamp(touch.pressure, 0f, 1f);

                // 左下角控制左 Flipper
                if (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 2)
                {
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Left flipper: touch begin with pressure: " + pressure);
                        leftFlipper.OnButtonPress(pressure);  // 根据压力控制左 Flipper
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Debug.Log("Left flipper: touch end");
                        leftFlipper.OnButtonRelease();  // 释放左 Flipper
                    }
                }

                // 右下角控制右 Flipper
                if (touch.position.x > Screen.width / 2 && touch.position.y < Screen.height / 2)
                {
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Right flipper: touch begin with pressure: " + pressure);
                        rightFlipper.OnButtonPress(pressure);  // 根据压力控制右 Flipper
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Debug.Log("Right flipper: touch end");
                        rightFlipper.OnButtonRelease();  // 释放右 Flipper
                    }
                }
            }
        }
    }
}

