using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections.Generic;

public class PlungerTrigger : MonoBehaviourPun
{
    float power;
    public float maxPower = 100f;
    private List<Rigidbody> ballList;
    private bool ballReady;
    private bool isPowerIncreasing;
    public Slider powerSlider;
    public Button launchButton;

    void Start()
    {
        if (launchButton == null)
        {
            Debug.LogError("Launch Button not assigned in the inspector!");
            return;
        }
        else
        {
            Debug.Log(launchButton);
        }

        if (powerSlider == null)
        {
            Debug.LogError("Power Slider not assigned in the inspector!");
            return;
        }

        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        ballList = new List<Rigidbody>();

        // 添加 EventTrigger 组件
        EventTrigger trigger = launchButton.gameObject.AddComponent<EventTrigger>();

        // 创建 PointerDown 事件条目
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        trigger.triggers.Add(pointerDownEntry);

        // 创建 PointerUp 事件条目
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        trigger.triggers.Add(pointerUpEntry);
    }

    void Update()
    {
        if (ballReady)
        {
            powerSlider.gameObject.SetActive(true);
        }
        else
        {
            powerSlider.gameObject.SetActive(false);
        }

        powerSlider.value = power;

        if (ballList.Count > 0)
        {
            ballReady = true;
            if (isPowerIncreasing) // 允许所有客户端增加 power
            {
                if (power <= maxPower)
                {
                    power += 50 * Time.deltaTime;
                    photonView.RPC("UpdateSliderValue", RpcTarget.All, power); // 将 power 广播到所有客户端
                }
            }
            if (!isPowerIncreasing && power > 0) // 松开按钮时发射小球
            {
                LaunchBall();
            }
        }
        else
        {
            ballReady = false;
            power = 0f;
        }
    }

    // PointerDown 事件的方法
    public void OnPointerDown(PointerEventData data)
    {
        isPowerIncreasing = true;
        Debug.Log("Launch button pressed: Power increasing started.");
        photonView.RPC("StartPowerIncreasing", RpcTarget.Others);
    }

    // PointerUp 事件的方法
    public void OnPointerUp(PointerEventData data)
    {
        isPowerIncreasing = false;
        Debug.Log("Launch button released: Power increasing stopped.");
        photonView.RPC("StopPowerIncreasing", RpcTarget.Others);
    }

    // RPC 方法来同步 power 增加的状态
    [PunRPC]
    void StartPowerIncreasing()
    {
        isPowerIncreasing = true;
    }

    [PunRPC]
    void StopPowerIncreasing()
    {
        isPowerIncreasing = false;
    }

    // RPC 方法来同步 Slider 值到所有客户端
    [PunRPC]
    public void UpdateSliderValue(float newPower)
    {
        power = newPower;
        powerSlider.value = power;
    }

    // 发射小球的方法，通过网络同步
    private void LaunchBall()
    {
        foreach (Rigidbody r in ballList)
        {
            PhotonView ballView = r.GetComponent<PhotonView>();
            if (ballView != null && !ballView.IsMine)
            {
                ballView.TransferOwnership(PhotonNetwork.LocalPlayer); // 转移球的所有权给当前客户端
            }

            r.AddForce(power * Vector3.forward, ForceMode.Impulse);
        }

        photonView.RPC("SyncLaunch", RpcTarget.Others, power); // 将发射动作同步到其他客户端
        power = 0f; // 发射后重置 power
    }

    [PunRPC]
    void SyncLaunch(float syncedPower)
    {
        foreach (Rigidbody r in ballList)
        {
            r.AddForce(syncedPower * Vector3.forward, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ballList.Add(other.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ballList.Remove(other.gameObject.GetComponent<Rigidbody>());
            power = 0f;
        }
    }
}
