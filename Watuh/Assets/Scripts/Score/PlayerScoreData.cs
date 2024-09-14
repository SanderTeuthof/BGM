using System.Collections.Generic;

[System.Serializable]
public class PlayerScoreData
{
    public string playerName;
    public List<LevelScoreData> levelScores;

    public PlayerScoreData(string playerName, List<LevelScoreData> levelScores)
    {
        this.playerName = playerName;
        this.levelScores = levelScores;
    }
}
