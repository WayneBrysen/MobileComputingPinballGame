using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RightFlipperControl : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float maxHitStrength = 1000f; // 最大弹起力度
    public float flipperDamper = 150f;
    HingeJoint hinge;
    private bool isPressed = false;
    private float pressure = 0f;  // 当前触摸压力

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // 按下时调用，根据触摸压力调节速度
    public void OnButtonPress(float touchPressure)
    {
        isPressed = true;
        pressure = touchPressure;  // 设置触摸压力
    }

    // 松开时调用
    public void OnButtonRelease()
    {
        isPressed = false;
        pressure = 0f;  // 重置压力
    }

    void Update()
    {
        JointSpring spring = new JointSpring
        {
            spring = maxHitStrength + 1000 * pressure,  // 根据压力调整力度
            damper = flipperDamper
        };
   
        Debug.Log("Right flipper: HitStrength: " + spring.spring);

        if (isPressed)
        {
            spring.targetPosition = pressedPosition;
        }
        else
        {
            spring.targetPosition = restPosition;
        }

        hinge.spring = spring;
        hinge.useLimits = true;
    }
}


