using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameSetter : MonoBehaviour
{
    private PlayerScoreData _playerScore;
    private void Start()
    {
        _playerScore = ScoreDataLoader.LoadPlayerScoreData();
    }

    // This method is called when the user submits their name (e.g., through a UI button)
    public void OnSubmitName(string name)
    {

        if (!string.IsNullOrEmpty(name))
        {
            _playerScore.playerName = name;
            ScoreDataWriter.WritePlayerScoreData(_playerScore);
            Debug.Log(ScoreDataLoader.LoadPlayerScoreData().playerName);
        }
        else
        {
            Debug.LogWarning("Name input field is empty!");
        }
    }
}

