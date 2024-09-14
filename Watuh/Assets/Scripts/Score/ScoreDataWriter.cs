using UnityEngine;

public class ScoreDataWriter
{
    // Save player score data to PlayerPrefs
    public static void WritePlayerScoreData(PlayerScoreData playerScoreData)
    {
        string jsonData = JsonUtility.ToJson(playerScoreData, true);
        PlayerPrefs.SetString("PlayerScoreData", jsonData);
        PlayerPrefs.Save();
    }
}
