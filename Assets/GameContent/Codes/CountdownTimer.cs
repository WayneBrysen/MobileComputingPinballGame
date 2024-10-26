using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using TMPro;

public class CountdownTimer : MonoBehaviourPunCallbacks
{
    public float countdownTime = 60f; // 倒计时时间，单位：秒
    private float currentTime;
    public TextMeshProUGUI countdownText;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentTime = countdownTime;
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            photonView.RPC("UpdateCountdown", RpcTarget.All, currentTime);
            yield return null;
        }

        // 倒计时结束，执行游戏结束逻辑
        photonView.RPC("OnCountdownEnd", RpcTarget.All);
    }

    [PunRPC]
    void UpdateCountdown(float time)
    {
        currentTime = time;
        int intTime = Mathf.CeilToInt(currentTime);
        countdownText.text = "CountDown: " + intTime.ToString();
    }

    [PunRPC]
    void OnCountdownEnd()
    {
        Debug.Log("倒计时结束，游戏结束！");

        StartCoroutine(LoadScoreboardScene());

    }

    IEnumerator LoadScoreboardScene()
    {
        yield return new WaitForSeconds(1f);

        // 加载 Scoreboard 场景
        PhotonNetwork.LoadLevel("Scoreboard");
    }
}