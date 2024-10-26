using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI myScoreText;         // 显示自己的分数
    public TextMeshProUGUI opponentScoreText;   // 显示对手的分数
    private int score = 0;                      // 自己的分数

    void Start()
    {
        // 初始化自己的分数显示
        myScoreText.text = PhotonNetwork.NickName + " Score: 0";

        // 初始化对手的分数显示
        int opponentScore = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                string opponentName = player.NickName;

                // 检查对手是否已经有分数记录
                if (player.CustomProperties.ContainsKey("PlayerScore"))
                {
                    opponentScore = (int)player.CustomProperties["PlayerScore"];
                }

                opponentScoreText.text = opponentName + " Score: " + opponentScore.ToString();
                break;  // 假设只有一个对手，跳出循环
            }
        }
    }

    // 增加分数并更新UI
    public void AddScore(int points)
    {
        score += points;
        myScoreText.text = PhotonNetwork.NickName + " Score: " + score.ToString();

        // 更新Custom Properties
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "PlayerScore", score }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // 更新对手的分数显示
    void UpdateOpponentScore(int opponentScore)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                string opponentName = player.NickName;
                opponentScoreText.text = opponentName + " Score: " + opponentScore.ToString();
                break;
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 如果是对手的分数更新
        if (targetPlayer != PhotonNetwork.LocalPlayer && changedProps.ContainsKey("PlayerScore"))
        {
            int opponentScore = (int)changedProps["PlayerScore"];
            UpdateOpponentScore(opponentScore);
        }
    }
}