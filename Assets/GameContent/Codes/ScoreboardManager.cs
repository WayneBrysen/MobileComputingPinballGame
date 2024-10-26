using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    public GameObject scoreForRank1; // ScoreForRank_1 对象的引用
    public GameObject scoreForRank2; // ScoreForRank_2 对象的引用

    void Start()
    {
        // 等待所有玩家都加载了场景
        StartCoroutine(WaitForPlayersAndDisplayScores());
    }

    IEnumerator WaitForPlayersAndDisplayScores()
    {
        // 等待直到所有玩家都在房间中
        while (PhotonNetwork.PlayerList.Length < 2)
        {
            yield return null;
        }

        // 等待一小段时间，确保所有 Custom Properties 已更新
        yield return new WaitForSeconds(0.5f);

        // 显示分数
        DisplayScores();
    }

    void DisplayScores()
    {
        // 创建一个列表来存储玩家的分数数据
        List<PlayerScoreData> playerScores = new List<PlayerScoreData>();

        // 遍历所有玩家，获取他们的分数
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int score = 0;
            if (player.CustomProperties.ContainsKey("PlayerScore"))
            {
                score = (int)player.CustomProperties["PlayerScore"];
            }

            playerScores.Add(new PlayerScoreData(player.NickName, score));
        }

        // 根据分数从高到低排序
        playerScores.Sort((x, y) => y.score.CompareTo(x.score));

        // 更新 UI 显示
        if (playerScores.Count >= 1)
        {
            UpdateScoreUI(scoreForRank1, "1", playerScores[0]);
        }

        if (playerScores.Count >= 2)
        {
            UpdateScoreUI(scoreForRank2, "2", playerScores[1]);
        }
    }

    void UpdateScoreUI(GameObject scoreForRank, string rank, PlayerScoreData playerData)
    {
        TextMeshProUGUI rankText = scoreForRank.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = scoreForRank.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI scoreText = scoreForRank.transform.Find("Score").GetComponent<TextMeshProUGUI>();

        rankText.text = rank;
        nameText.text = playerData.nickName;
        scoreText.text = playerData.score.ToString();
    }

    // 辅助类，用于存储玩家数据
    class PlayerScoreData
    {
        public string nickName;
        public int score;

        public PlayerScoreData(string nickName, int score)
        {
            this.nickName = nickName;
            this.score = score;
        }
    }
}