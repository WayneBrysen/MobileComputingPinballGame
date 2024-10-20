using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RightFlipperControl : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float minHitStrength = 3000f;
    public float maxHitStrength = 20000f; // 最大弹起力度
    public float flipperDamper = 150f;
    public float smoothPressureSpeed = 5f;      // 压力平滑过渡的速度
    HingeJoint hinge;
    private bool isPressed = false;
    private float currentPressure = 0f;         // 当前的压力值
    private float targetPressure = 0f;          // 目标压力值
    private bool isDoublePoints = false;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // 按下时调用，根据触摸压力调节速度
    public void OnButtonPress(float touchPressure)
    {
        isPressed = true;
        targetPressure = touchPressure;  // 设置触摸压力
    }

    // 松开时调用
    public void OnButtonRelease()
    {
        isPressed = false;
        targetPressure = 0f;  // 重置压力
    }

    void Update()
    {
        currentPressure = Mathf.Lerp(currentPressure, targetPressure, Time.deltaTime * smoothPressureSpeed);
        // 计算弹簧力度，确保在合理范围内
        float springForce = Mathf.Clamp(minHitStrength + 15000 * currentPressure, 5000, 20000);

        JointSpring spring = new JointSpring
        {
            spring = springForce,  // 根据压力动态调整弹力
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

    public void SetDoublePoints(bool isActive)
    {
        isDoublePoints = isActive;
        Debug.Log("Right Flipper Double Points Mode: " + isActive);
    }

    // 获取是否启用了双倍积分模式
    public bool IsDoublePointsActive()
    {
        return isDoublePoints;
    }
}


