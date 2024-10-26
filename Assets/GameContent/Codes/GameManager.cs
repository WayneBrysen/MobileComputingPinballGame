using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;  // Required for scene management

public class GameManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int score = 0;
    private double startTime;
    private bool startTimer = false;
    private double timerDuration = 60.0; // Game duration in seconds
    private double timerIncrementValue;

    void Start()
    {
        scoreText.text = "Score: 0";
        timerText.text = "Time: 00:00";

        if (PhotonNetwork.IsMasterClient)
        {
            // Master client initializes and sets the start time
            startTime = PhotonNetwork.Time;
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

            startTimer = true;
            Debug.Log("MasterClient: Timer started at " + startTime);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("StartTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            startTimer = true;
            Debug.Log("Client: Retrieved start time as " + startTime);
        }
    }

    void Update()
    {
        if (!startTimer) return;

        timerIncrementValue = PhotonNetwork.Time - startTime;
        double timeRemaining = timerDuration - timerIncrementValue;

        if (timeRemaining > 0)
        {
            DisplayTime(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            startTimer = false;
            DisplayTime(timeRemaining);
            EndGame();
        }
    }

    void DisplayTime(double timeToDisplay)
    {
        int minutes = Mathf.FloorToInt((float)timeToDisplay / 60);
        int seconds = Mathf.FloorToInt((float)timeToDisplay % 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    void EndGame()
    {
        // Load the "Scoreboard" scene for all players
        PhotonNetwork.LoadLevel("Scoreboard");
    }

    // Adds points to the score and updates the score UI
    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score.ToString();
    }
}
