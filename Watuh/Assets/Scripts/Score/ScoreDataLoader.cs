using System.Collections.Generic;
using UnityEngine;

public class ScoreDataLoader
{
    // Load player score data from PlayerPrefs
    public static PlayerScoreData LoadPlayerScoreData()
    {
        string jsonData = PlayerPrefs.GetString("PlayerScoreData");
        if (!string.IsNullOrEmpty(jsonData))
        {
            PlayerScoreData playerScoreData = JsonUtility.FromJson<PlayerScoreData>(jsonData);
            return playerScoreData;
        }
        else
        {
            Debug.LogError("No player score data found in PlayerPrefs.");
            return new PlayerScoreData("DefaultPlayer", new List<LevelScoreData>());
        }
    }
}