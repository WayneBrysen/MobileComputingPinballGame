using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // 引入EventSystems以处理事件触发器

public class FlipperMobile : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float hitStrenght = 10000f;
    public float flipperDamper = 150f;
    HingeJoint hinge;

    private bool isPressed = false;  // 判断按钮是否按下

    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // 当按钮按下时的回调
    public void OnButtonPress()
    {
        isPressed = true;  // 按下按钮
    }

    // 当按钮松开时的回调
    public void OnButtonRelease()
    {
        isPressed = false;  // 松开按钮
    }

    // Update is called once per frame
    void Update()
    {
        JointSpring spring = new JointSpring();
        spring.spring = hitStrenght;
        spring.damper = flipperDamper;

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