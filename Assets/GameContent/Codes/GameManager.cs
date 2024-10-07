using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;  
    private int score = 0;

    void Start()
    {
        scoreText.text = "Score: 0";
    }

    // 增加分数并更新UI
    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score.ToString();
    }
}