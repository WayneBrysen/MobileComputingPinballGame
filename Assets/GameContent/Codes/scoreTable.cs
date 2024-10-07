using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreDisplay : MonoBehaviour {

    private Transform scoresContainer;
    private Transform templateEntry;
    private List<ScoreEntry> scoreEntryList;
    private List<Transform> scoreTransformList;

    private void Awake() {
        // Find and cache the container that holds score entries
        scoresContainer = transform.Find("scores");
        Debug.Log(scoresContainer != null ? "Score container found!" : "Score container missing!");
        templateEntry = scoresContainer.Find("scoreTemplate");
        Debug.Log(templateEntry != null ? "Template found!" : "Template missing!");

        // Hide the template initially
        templateEntry.gameObject.SetActive(false);

        // Create a sample list of high scores
        scoreEntryList = new List<ScoreEntry>() {
            new ScoreEntry{ scoreValue = 121236, playerName = "CJ" },
            new ScoreEntry{ scoreValue = 321224, playerName = "John Doe" },
            new ScoreEntry{ scoreValue = 432322, playerName = "Mario" },
            new ScoreEntry{ scoreValue = 634323, playerName = "Chungus" },
            new ScoreEntry{ scoreValue = 324872, playerName = "Big Smoke" },
            new ScoreEntry{ scoreValue = 854324, playerName = "MJ" },
            new ScoreEntry{ scoreValue = 936465, playerName = "Dave Free" }
        };

        // Sort the list by score in descending order
        scoreEntryList.Sort((a, b) => b.scoreValue.CompareTo(a.scoreValue));

        // Instantiate score entries and store them
        scoreTransformList = new List<Transform>();
        foreach (ScoreEntry entry in scoreEntryList) {
            AddScoreEntry(entry, scoresContainer, scoreTransformList);
        }
    }

    private void AddScoreEntry(ScoreEntry entry, Transform container, List<Transform> transformList) {
        float entryHeight = 30f;

        // Instantiate and position new entry
        Transform entryTransform = Instantiate(templateEntry, container);
        RectTransform rectTransform = entryTransform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -entryHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        // Determine rank and format it
        int rank = transformList.Count + 1;
        string rankLabel = rank switch {
            1 => "1ST",
            2 => "2ND",
            3 => "3RD",
            _ => rank + "TH",
        };

        // Update UI elements for rank, score, and name
        entryTransform.Find("rankTxt").GetComponent<Text>().text = rankLabel;
        entryTransform.Find("scoreTxt").GetComponent<Text>().text = entry.scoreValue.ToString();
        entryTransform.Find("nameTxt").GetComponent<Text>().text = entry.playerName;

        // Add to the list of score transforms
        transformList.Add(entryTransform);
    }

    private class ScoreEntry {
        public int scoreValue;
        public string playerName;
    }
}
