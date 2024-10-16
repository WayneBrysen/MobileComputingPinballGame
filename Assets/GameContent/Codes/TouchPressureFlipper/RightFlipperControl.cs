using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RightFlipperControl : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float maxHitStrength = 1000f; // ���������
    public float flipperDamper = 150f;
    HingeJoint hinge;
    private bool isPressed = false;
    private float pressure = 0f;  // ��ǰ����ѹ��

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // ����ʱ���ã����ݴ���ѹ�������ٶ�
    public void OnButtonPress(float touchPressure)
    {
        isPressed = true;
        pressure = touchPressure;  // ���ô���ѹ��
    }

    // �ɿ�ʱ����
    public void OnButtonRelease()
    {
        isPressed = false;
        pressure = 0f;  // ����ѹ��
    }

    void Update()
    {
        JointSpring spring = new JointSpring
        {
            spring = maxHitStrength + 1000 * pressure,  // ����ѹ����������
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


