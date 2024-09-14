using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScoreManager : MonoBehaviour
{
    [SerializeField]
    private FloatReference _timer;

    private PlayerScoreData playerScoreData;

    private void Start()
    {
        // Load player score data at the start
        playerScoreData = ScoreDataLoader.LoadPlayerScoreData();

        // Get the current scene name as the level name
        string currentLevelName = SceneManager.GetActiveScene().name;

        // Check if the level exists or add it with default values
        CheckOrAddLevel(currentLevelName);

        DebugCurrentLevelScore();
    }

    // Check if level exists in the score data, add if not, and increment tries
    private void CheckOrAddLevel(string levelName)
    {
        LevelScoreData? existingLevelScore = FindLevelScore(levelName);

        if (existingLevelScore.HasValue)
        {
            // If level is found, increment the tries
            LevelScoreData updatedScore = existingLevelScore.Value;
            updatedScore.tries += 1;
            UpdateLevelScore(updatedScore);
        }
        else
        {
            // If level is not found, add a new entry with default values
            LevelScoreData newLevelScore = new LevelScoreData(levelName, 1200f, 1); // Default time: 1200 seconds (20 minutes)
            playerScoreData.levelScores.Add(newLevelScore);
        }

        // Save updated score data back to PlayerPrefs
        ScoreDataWriter.WritePlayerScoreData(playerScoreData);
    }

    // Find the level score in the list by name
    private LevelScoreData? FindLevelScore(string levelName)
    {
        foreach (LevelScoreData score in playerScoreData.levelScores)
        {
            if (score.levelName == levelName)
            {
                return score;
            }
        }
        return null;
    }

    // Replace the old level score with the updated one
    private void UpdateLevelScore(LevelScoreData updatedScore)
    {
        for (int i = 0; i < playerScoreData.levelScores.Count; i++)
        {
            if (playerScoreData.levelScores[i].levelName == updatedScore.levelName)
            {
                playerScoreData.levelScores[i] = updatedScore;
                break;
            }
        }
    }

    public void LevelEnd(Component sender, object data)
    {
        // Get the current level name
        string currentLevelName = SceneManager.GetActiveScene().name;

        // Find the level score
        LevelScoreData? existingLevelScore = FindLevelScore(currentLevelName);

        if (existingLevelScore.HasValue)
        {
            // If the new time is smaller, update the best time
            LevelScoreData levelScore = existingLevelScore.Value;
            if (_timer.value < levelScore.bestTime)
            {
                levelScore.bestTime = _timer.value;
                UpdateLevelScore(levelScore);

                // Save the updated score data back to PlayerPrefs
                ScoreDataWriter.WritePlayerScoreData(playerScoreData);
            }
        }
        DebugCurrentLevelScore();
    }

    public void DebugCurrentLevelScore()
    {
        // Get the current scene name as the level name
        string currentLevelName = SceneManager.GetActiveScene().name;

        // Find the level score for the current level
        LevelScoreData? levelScore = FindLevelScore(currentLevelName);

        if (levelScore.HasValue)
        {
            Debug.Log($"Level: {levelScore.Value.levelName}, Best Time: {levelScore.Value.bestTime}, Tries: {levelScore.Value.tries}");
        }
        else
        {
            Debug.LogWarning($"No score data found for level: {currentLevelName}");
        }
    }
}
