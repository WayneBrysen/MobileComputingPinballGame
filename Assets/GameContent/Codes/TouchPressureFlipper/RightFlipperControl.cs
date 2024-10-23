using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RightFlipperControl : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float minHitStrength = 3000f;
    public float maxHitStrength = 20000f; // ���������
    public float flipperDamper = 150f;
    public float smoothPressureSpeed = 5f;      // ѹ��ƽ�����ɵ��ٶ�
    HingeJoint hinge;
    private bool isPressed = false;
    private float currentPressure = 0f;         // ��ǰ��ѹ��ֵ
    private float targetPressure = 0f;          // Ŀ��ѹ��ֵ
    private bool isDoublePoints = false;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // ����ʱ���ã����ݴ���ѹ�������ٶ�
    public void OnButtonPress(float touchPressure)
    {
        isPressed = true;
        targetPressure = touchPressure;  // ���ô���ѹ��
    }

    // �ɿ�ʱ����
    public void OnButtonRelease()
    {
        isPressed = false;
        targetPressure = 0f;  // ����ѹ��
    }

    void Update()
    {
        currentPressure = Mathf.Lerp(currentPressure, targetPressure, Time.deltaTime * smoothPressureSpeed);
        // ���㵯�����ȣ�ȷ���ں���Χ��
        float springForce = Mathf.Clamp(minHitStrength + 15000 * currentPressure, 5000, 20000);

        JointSpring spring = new JointSpring
        {
            spring = springForce,  // ����ѹ����̬��������
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

    // ��ȡ�Ƿ�������˫������ģʽ
    public bool IsDoublePointsActive()
    {
        return isDoublePoints;
    }
}


