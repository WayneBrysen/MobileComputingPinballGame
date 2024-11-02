using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    public GameObject scoreForRank1;
    public GameObject scoreForRank2;

    public TextMeshProUGUI finalScoreText; // Reference to the UI Text object for displaying the final score
    public TextMeshProUGUI scoreOnlyText;

    void Start()
    {
        StartCoroutine(WaitForPlayersAndDisplayScores());
        
        DisplayFinalScore();
    }

    IEnumerator WaitForPlayersAndDisplayScores()
    {
        while (PhotonNetwork.PlayerList.Length < 2)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        DisplayScores();
    }
    void DisplayFinalScore()
    {
        int placeholderScore = 1234; 
        // Retrieve the final score from PlayerPrefs and display it
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0); // Default to 0 if no score found
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore.ToString();
        }
        if (scoreOnlyText != null)
        {
            scoreOnlyText.text = finalScore.ToString();
        }
    }

    void DisplayScores()
    {
        List<PlayerScoreData> playerScores = new List<PlayerScoreData>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int score = 0;
            if (player.CustomProperties.ContainsKey("PlayerScore"))
            {
                score = (int)player.CustomProperties["PlayerScore"];
            }

            playerScores.Add(new PlayerScoreData(player.NickName, score));
        }

        playerScores.Sort((x, y) => y.score.CompareTo(x.score));

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