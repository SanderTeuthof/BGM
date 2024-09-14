using TMPro;
using UnityEngine;

public class LevelScoreDisplay : MonoBehaviour
{
    [SerializeField] 
    private string _levelSceneName;     

    private TextMeshProUGUI _scoreText;    

    private PlayerScoreData _playerScoreData;

    private void Start()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();   

        _playerScoreData = ScoreDataLoader.LoadPlayerScoreData();

        DisplayLevelScore(_levelSceneName);
    }

    private void DisplayLevelScore(string levelName)
    {
        // Find the level score for the specified level name
        LevelScoreData? levelScore = FindLevelScore(levelName);

        if (levelScore.HasValue)
        {
            // Get the best time and number of tries from the level score data
            float bestTime = levelScore.Value.bestTime;
            int tries = levelScore.Value.tries;

            // Format the time into minutes:seconds:hundredths
            string formattedTime = FormatTime(bestTime);

            // Set the TextMeshPro text to show the formatted time and tries
            _scoreText.text = $"Best Time: {formattedTime}\nTries: {tries}";
        }
        else
        {
            // If no score data found, display a default message
            _scoreText.text = $"Not played yet!";
        }
    }

    // Find the level score data by name
    private LevelScoreData? FindLevelScore(string levelName)
    {
        foreach (LevelScoreData score in _playerScoreData.levelScores)
        {
            if (score.levelName == levelName)
            {
                return score;
            }
        }
        return null;  // Return null if level data is not found
    }

    // Format the time as "minutes:seconds:hundredths"
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);         // Calculate minutes
        int seconds = Mathf.FloorToInt(time % 60);         // Calculate seconds
        int hundredths = Mathf.FloorToInt((time * 100) % 100);  // Calculate hundredths of a second

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
    }
}
