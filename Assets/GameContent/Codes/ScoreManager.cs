using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    // Serialized fields for UI elements
    [SerializeField]
    private TextMeshProUGUI inputScore; // Displays the score input field (can be updated to a different UI type if needed)

    [SerializeField]
    private TMP_InputField inputName; // Input field for the user's name

    // Event to be invoked when a score is submitted
    public UnityEvent<string, int> submitScoreEvent;

    /// <summary>
    /// Submits the score entered by the user if it is a valid number.
    /// </summary>
    public void SubmitScore() 
    {
        int score; // Variable to hold the parsed score value

        // Attempt to parse the score input text as an integer
        if (int.TryParse(inputScore.text, out score))
        {
            // Successfully parsed the score, invoke the event with the user's name and score
            submitScoreEvent.Invoke(inputName.text, score);
        }
        else
        {
            // Log an error message if parsing fails, indicating an invalid input
            Debug.LogError("Invalid score input. Please enter a valid number.");
            // Optionally, display an error message to the user in the UI
        }
    }
}
