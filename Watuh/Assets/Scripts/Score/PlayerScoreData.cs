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

    public float TotalTime()
    {
        float time = 0;

        foreach (var levelScore in levelScores)
            time += levelScore.bestTime;

        return time;
    }
}
