using System.Collections.Generic;

[System.Serializable]
public class PlayerScoreData
{
    public string playerName; // The player's name
    public List<LevelScoreData> levelScores; // A list of scores for each level

    public PlayerScoreData(string playerName, List<LevelScoreData> levelScores)
    {
        this.playerName = playerName;
        this.levelScores = levelScores;
    }
}
