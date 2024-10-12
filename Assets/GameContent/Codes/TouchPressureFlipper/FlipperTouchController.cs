using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FlipperTouchController : MonoBehaviour
{
    // �������� Flipper ������
    public LeftFlipperControl leftFlipper;
    public RightFlipperControl rightFlipper;

    void Update()
    {
        // ����Ƿ��д�������
        if (Input.touchCount > 0)
        {
            // �������д���
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                // ��ȡ����ѹ����ѹ����Χͨ��Ϊ0.0��1.0��ĳЩ�豸����֧�ָ��ߵ�ѹ��ֵ
                float pressure = Mathf.Clamp(touch.pressure, 0f, 1f);

                // ���½ǿ����� Flipper
                if (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 2)
                {
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Left flipper: touch begin with pressure: " + pressure);
                        leftFlipper.OnButtonPress(pressure);  // ����ѹ�������� Flipper
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Debug.Log("Left flipper: touch end");
                        leftFlipper.OnButtonRelease();  // �ͷ��� Flipper
                    }
                }

                // ���½ǿ����� Flipper
                if (touch.position.x > Screen.width / 2 && touch.position.y < Screen.height / 2)
                {
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Right flipper: touch begin with pressure: " + pressure);
                        rightFlipper.OnButtonPress(pressure);  // ����ѹ�������� Flipper
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Debug.Log("Right flipper: touch end");
                        rightFlipper.OnButtonRelease();  // �ͷ��� Flipper
                    }
                }
            }
        }
    }
}

