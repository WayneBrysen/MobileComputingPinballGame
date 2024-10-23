using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FlipperTouchController : MonoBehaviour
{
    // 引用左、右 Flipper 控制器
    public LeftFlipperControl leftFlipper;
    public RightFlipperControl rightFlipper;

    public AudioSource audioSource;   // 音频源，用于播放颤音
    public float maxVolume = 1.0f;    // 最大音量
    public float smoothSpeed = 5.0f;  // 音量平滑变化的速度
    private float targetVolume = 0f;  // 目标音量
    private float currentVolume = 0f; // 当前音量，用于平滑过渡

    public Color normalColor = Color.white;   // Flipper 的正常颜色
    public Color highlightColor = Color.red;  // Flipper 达到条件后变红

    private Renderer leftFlipperRenderer;
    private Renderer rightFlipperRenderer;

    private float volumeTimer = 0f;           // 计时器，用来记录音量达到9.99后的持续时间
    private float volumeThreshold = 0.99f;    // 音量阈值，0.99
    private float timeThreshold = 5f;         // 时间阈值，5秒
    private float doublePointsTimer = 0f;     // 双倍积分模式的持续时间
    private bool isDoublePointsActive = false; // 是否处于双倍积分模式

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("Missing AudioSource component.");
                enabled = false;  // 禁用脚本，避免后续操作
                return;
            }
        }

        leftFlipperRenderer = leftFlipper.GetComponent<Renderer>();
        rightFlipperRenderer = rightFlipper.GetComponent<Renderer>();

        // 初始化 Flipper 颜色为正常色
        leftFlipperRenderer.material.color = normalColor;
        rightFlipperRenderer.material.color = normalColor;
    }

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

                // 将压力值映射到音量
                targetVolume = Mathf.Lerp(0f, maxVolume, pressure);

                // 平滑地调整当前音量
                currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);

                // 设置音频源的音量
                audioSource.volume = currentVolume;

                Debug.Log("Pressure: " + pressure + ", Volume: " + audioSource.volume);

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
        else
        {
            // 当没有触摸时，音量平滑归零
            targetVolume = 0f;
            currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);
            audioSource.volume = currentVolume;
        }

        // 如果音量达到 0.99，开始计时
        if (audioSource.volume >= volumeThreshold && !isDoublePointsActive)
        {
            volumeTimer += Time.deltaTime;

            // 当音量持续 5 秒时，启动双倍积分模式并改变 Flipper 的颜色为红色
            if (volumeTimer >= timeThreshold)
            {
                ActivateDoublePointsMode();
            }
        }
        else if (audioSource.volume < volumeThreshold && !isDoublePointsActive)
        {
            // 如果音量小于 0.99，重置计时器
            volumeTimer = 0f;
        }

        // 如果双倍积分模式激活，开始计时
        if (isDoublePointsActive)
        {
            doublePointsTimer += Time.deltaTime;

            // 持续 5 秒后，恢复原始颜色和普通积分模式
            if (doublePointsTimer >= timeThreshold)
            {
                DeactivateDoublePointsMode();
            }
        }
    }
    // 激活双倍积分模式并将 Flipper 变为红色
    void ActivateDoublePointsMode()
    {
        isDoublePointsActive = true;
        doublePointsTimer = 0f; // 重置双倍积分模式的计时器

        leftFlipperRenderer.material.color = highlightColor;
        rightFlipperRenderer.material.color = highlightColor;

        leftFlipper.SetDoublePoints(true);
        rightFlipper.SetDoublePoints(true);

        Debug.Log("Double Points Mode Activated");
    }

    // 停用双倍积分模式并恢复原始颜色
    void DeactivateDoublePointsMode()
    {
        isDoublePointsActive = false;
        volumeTimer = 0f; // 重置音量计时器

        leftFlipperRenderer.material.color = normalColor;
        rightFlipperRenderer.material.color = normalColor;

        leftFlipper.SetDoublePoints(false);
        rightFlipper.SetDoublePoints(false);

        Debug.Log("Double Points Mode Deactivated");
    }
}

