using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FlipperTouchController : MonoBehaviour
{
    // �������� Flipper ������
    public LeftFlipperControl leftFlipper;
    public RightFlipperControl rightFlipper;

    public AudioSource audioSource;   // ��ƵԴ�����ڲ��Ų���
    public float maxVolume = 1.0f;    // �������
    public float smoothSpeed = 5.0f;  // ����ƽ���仯���ٶ�
    private float targetVolume = 0f;  // Ŀ������
    private float currentVolume = 0f; // ��ǰ����������ƽ������

    public Color normalColor = Color.white;   // Flipper ��������ɫ
    public Color highlightColor = Color.red;  // Flipper �ﵽ��������

    private Renderer leftFlipperRenderer;
    private Renderer rightFlipperRenderer;

    private float volumeTimer = 0f;           // ��ʱ����������¼�����ﵽ9.99��ĳ���ʱ��
    private float volumeThreshold = 0.99f;    // ������ֵ��0.99
    private float timeThreshold = 5f;         // ʱ����ֵ��5��
    private float doublePointsTimer = 0f;     // ˫������ģʽ�ĳ���ʱ��
    private bool isDoublePointsActive = false; // �Ƿ���˫������ģʽ

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("Missing AudioSource component.");
                enabled = false;  // ���ýű��������������
                return;
            }
        }

        leftFlipperRenderer = leftFlipper.GetComponent<Renderer>();
        rightFlipperRenderer = rightFlipper.GetComponent<Renderer>();

        // ��ʼ�� Flipper ��ɫΪ����ɫ
        leftFlipperRenderer.material.color = normalColor;
        rightFlipperRenderer.material.color = normalColor;
    }

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

                // ��ѹ��ֵӳ�䵽����
                targetVolume = Mathf.Lerp(0f, maxVolume, pressure);

                // ƽ���ص�����ǰ����
                currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);

                // ������ƵԴ������
                audioSource.volume = currentVolume;

                Debug.Log("Pressure: " + pressure + ", Volume: " + audioSource.volume);

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
        else
        {
            // ��û�д���ʱ������ƽ������
            targetVolume = 0f;
            currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);
            audioSource.volume = currentVolume;
        }

        // ��������ﵽ 0.99����ʼ��ʱ
        if (audioSource.volume >= volumeThreshold && !isDoublePointsActive)
        {
            volumeTimer += Time.deltaTime;

            // ���������� 5 ��ʱ������˫������ģʽ���ı� Flipper ����ɫΪ��ɫ
            if (volumeTimer >= timeThreshold)
            {
                ActivateDoublePointsMode();
            }
        }
        else if (audioSource.volume < volumeThreshold && !isDoublePointsActive)
        {
            // �������С�� 0.99�����ü�ʱ��
            volumeTimer = 0f;
        }

        // ���˫������ģʽ�����ʼ��ʱ
        if (isDoublePointsActive)
        {
            doublePointsTimer += Time.deltaTime;

            // ���� 5 ��󣬻ָ�ԭʼ��ɫ����ͨ����ģʽ
            if (doublePointsTimer >= timeThreshold)
            {
                DeactivateDoublePointsMode();
            }
        }
    }
    // ����˫������ģʽ���� Flipper ��Ϊ��ɫ
    void ActivateDoublePointsMode()
    {
        isDoublePointsActive = true;
        doublePointsTimer = 0f; // ����˫������ģʽ�ļ�ʱ��

        leftFlipperRenderer.material.color = highlightColor;
        rightFlipperRenderer.material.color = highlightColor;

        leftFlipper.SetDoublePoints(true);
        rightFlipper.SetDoublePoints(true);

        Debug.Log("Double Points Mode Activated");
    }

    // ͣ��˫������ģʽ���ָ�ԭʼ��ɫ
    void DeactivateDoublePointsMode()
    {
        isDoublePointsActive = false;
        volumeTimer = 0f; // ����������ʱ��

        leftFlipperRenderer.material.color = normalColor;
        rightFlipperRenderer.material.color = normalColor;

        leftFlipper.SetDoublePoints(false);
        rightFlipper.SetDoublePoints(false);

        Debug.Log("Double Points Mode Deactivated");
    }
}

