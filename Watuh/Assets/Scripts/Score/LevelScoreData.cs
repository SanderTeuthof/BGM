[System.Serializable]
public struct LevelScoreData
{
    public string levelName;  // The name of the level
    public float bestTime;    // The best time achieved for this level
    public int tries;         // The number of attempts taken

    public LevelScoreData(string levelName, float bestTime, int tries)
    {
        this.levelName = levelName;
        this.bestTime = bestTime;
        this.tries = tries;
    }
}
