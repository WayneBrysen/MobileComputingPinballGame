using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main; 

public class Leaderboard : MonoBehaviour
{
    // Serialized fields for displaying leaderboard names and scores
    [SerializeField]
    private List<TextMeshProUGUI> names; // List of UI elements to display player names

    [SerializeField]
    private List<TextMeshProUGUI> scores; // List of UI elements to display player scores

    // Public leaderboard key for accessing the leaderboard data
    private string publicLeaderboardKey = 
        "3da13e57dda00f2cf3e7868a9af775ff4b17e84adbb6b04ce6bfd4f9ce293877";

  
    public void GetLeaderboard() 
    {
        // Fetch leaderboard data using an external library (Dan.Main)
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, (msg) =>
        {
            // Loop through the UI elements to update the displayed names and scores
            for (int i = 0; i < names.Count; ++i) 
            {
                // Ensure we don't exceed the number of entries received from the leaderboard
                if (i < msg.Length)
                {
                    // Update the UI elements with player name and score
                    names[i].text = msg[i].Username;
                    scores[i].text = msg[i].Score.ToString();
                }
            }
        });
    }


    public void SetLeaderboardEntry(string username, int score) 
    {
        // Upload a new leaderboard entry using an external library (Dan.Main)
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, (msg) => 
        {
            // Refresh the leaderboard after successfully uploading the new entry
            GetLeaderboard();
        });
    }
}
