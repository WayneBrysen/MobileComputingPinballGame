using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;

    [SerializeField]
    private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = 
        "3da13e57dda00f2cf3e7868a9af775ff4b17e84adbb6b04ce6bfd4f9ce293877";

    public void GetLeaderboard() 
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, (msg) =>
        {
            for (int i = 0; i < names.Count; ++i) 
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        });
    }

    public void SetLeaderboardEntry(string username, int score) 
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, (msg) => 
        {

            GetLeaderboard();
        });
    }
}
