using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI myScoreText;
    public TextMeshProUGUI opponentScoreText;

    [SerializeField]
    private TextMeshProUGUI scoreObjectText; // Reference to the "Score" object in the scene

    private int score = 0;

    void Start()
    {
        myScoreText.text = PhotonNetwork.NickName + " Score: 0";

        int opponentScore = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                string opponentName = player.NickName;

                if (player.CustomProperties.ContainsKey("PlayerScore"))
                {
                    opponentScore = (int)player.CustomProperties["PlayerScore"];
                }

                opponentScoreText.text = opponentName + " Score: " + opponentScore.ToString();
                break;
            }
        }
        // Initialize the "Score" object in the scene
        if (scoreObjectText != null)
        {
        scoreObjectText.text = "Score: 0";
        }

    }

    public void AddScore(int points)
    {
        score += points;
        myScoreText.text = PhotonNetwork.NickName + " Score: " + score.ToString();

        // Update the "Score" object in the scene
        if (scoreObjectText != null)
        {
            scoreObjectText.text = "Score: " + score.ToString();
        }


        // ����Custom Properties
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "PlayerScore", score }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
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
    public void SaveFinalScore()
    {
    PlayerPrefs.SetInt("FinalScore", score); // Save the score to PlayerPrefs
    PlayerPrefs.Save(); // Ensure the score is saved
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer != PhotonNetwork.LocalPlayer && changedProps.ContainsKey("PlayerScore"))
        {
            int opponentScore = (int)changedProps["PlayerScore"];
            UpdateOpponentScore(opponentScore);
        }
    }
}