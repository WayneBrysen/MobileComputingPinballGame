using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // ����EventSystems�Դ����¼�������

public class FlipperMobile : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float hitStrenght = 10000f;
    public float flipperDamper = 150f;
    HingeJoint hinge;

    private bool isPressed = false;  // �жϰ�ť�Ƿ���

    // Start is called before the first frame update
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // ����ť����ʱ�Ļص�
    public void OnButtonPress()
    {
        isPressed = true;  // ���°�ť
    }

    // ����ť�ɿ�ʱ�Ļص�
    public void OnButtonRelease()
    {
        isPressed = false;  // �ɿ���ť
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