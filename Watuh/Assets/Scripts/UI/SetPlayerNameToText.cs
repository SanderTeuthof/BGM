using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetPlayerNameToText : MonoBehaviour
{
    private TextMeshProUGUI _nameText;

    private PlayerScoreData _playerScoreData;

    private void Start()
    {
        _nameText = GetComponent<TextMeshProUGUI>();

        _playerScoreData = ScoreDataLoader.LoadPlayerScoreData();

        DisplayName();
    }

    private void DisplayName()
    {
        _nameText.text = $"Go fast {_playerScoreData.playerName}!";
    }
}
