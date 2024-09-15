using Newtonsoft.Json;
using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderboardCommunicator : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] _topPositions;
    [SerializeField]
    private TextMeshProUGUI[] _aroundPlayerPositions;
    [SerializeField]
    private TextMeshProUGUI _bottomScore;
    [SerializeField]
    private int _numberOfLevels;

    private bool _signedIn = false;

    private string leaderboardID = "TestLeaaderBoardd";

    PlayerScoreData _playerData;


    public async void Awake()
    {
        _playerData = ScoreDataLoader.LoadPlayerScoreData();

        InitializationOptions options = new InitializationOptions();
        options.SetProfile("banana");

        await UnityServices.InitializeAsync(options);


        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _signedIn = true;

        //AddScore();
        //GetPaginatedScores(0,4);
        SetScores();
    }

    private async void SetScores()
    {
        if (_playerData.levelScores.Count == _numberOfLevels)
        {
            Debug.Log(_playerData.levelScores.Count);
            PlayerScoreData player = ScoreDataLoader.LoadPlayerScoreData();
            await AuthenticationService.Instance.UpdatePlayerNameAsync(player.playerName);
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("TestLeaaderBoardd", _playerData.TotalTime());
        }

        SetTopScores();
        SetBottomScore();
        SetPlayerScores();
    }

    private async void SetTopScores()
    {
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { Offset = 0, Limit = _topPositions.Length });
            if (scoresResponse != null && scoresResponse.Results.Count > 0)
            {
                for (int i = 0; i < scoresResponse.Results.Count; i++)
                {
                    var score = scoresResponse.Results[i];
                    TimeSpan time = TimeSpan.FromSeconds(score.Score); 
                    string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
                    string playerName = score.PlayerName.Split("#")[0];
                    playerName = playerName.Length > 12 ? playerName.Substring(0, 12) + ".." : playerName;

                    _topPositions[i].text = $"#{score.Rank + 1} {playerName} {formattedTime}";
                }
            }
        }
        catch (LeaderboardsException ex)
        {
            Debug.LogError($"Error fetching top scores: {ex.Message}");
        }
    }

    private async void SetBottomScore()
    {
        int stepSize = 100;  // Start with large step size
        int offset = 0;
        int totalPlayers = 0;

        while (stepSize >= 1)
        {
            try
            {
                var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { Offset = offset, Limit = 1 });

                if (scoresResponse.Results.Count > 0)
                {
                    // If there's a result, it means there are players at this rank, so we move forward
                    totalPlayers = scoresResponse.Results[0].Rank; // Save the last found rank as total players
                    offset += stepSize;  // Move to the next range
                }
                else
                {
                    // If no result is found, reduce the step size and search within the smaller range
                    offset -= stepSize;  // Step back by the current step size
                    stepSize /= 10;  // Reduce step size by 10
                    offset += stepSize;  // Adjust offset for the smaller range
                }
            }
            catch (LeaderboardsException ex)
            {
                Debug.LogError($"Error fetching leaderboard data: {ex.Message}");
                break;
            }
        }

        var worstScoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { Offset = totalPlayers, Limit = 1 });

        if (worstScoresResponse != null && worstScoresResponse.Results.Count > 0)
        {
            var score = worstScoresResponse.Results[0];

            TimeSpan time = TimeSpan.FromSeconds(score.Score);
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
            string playerName = score.PlayerName.Split("#")[0];
            playerName = playerName.Length > 12 ? playerName.Substring(0, 12) + ".." : playerName;

            _bottomScore.text = $"#{score.Rank + 1} {playerName} {formattedTime}";
        }

    }

    private async void SetPlayerScores()
    {
        try
        {
            if (_playerData.levelScores.Count == _numberOfLevels)
            {
                PlayerScoreData player = ScoreDataLoader.LoadPlayerScoreData();
                await AuthenticationService.Instance.UpdatePlayerNameAsync(player.playerName);
                var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("TestLeaaderBoardd", player.TotalTime());

                if (playerEntry != null)
                {
                    int playerRank = playerEntry.Rank;

                    if (playerRank < _topPositions.Length)
                    {
                        var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { Offset = _topPositions.Length, Limit = _aroundPlayerPositions.Length });
                        if (scores != null && scores.Results.Count > 0)
                        {
                            for (int i = 0; i < scores.Results.Count; i++)
                            {
                                var score = scores.Results[i];
                                TimeSpan time = TimeSpan.FromSeconds(score.Score);
                                string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
                                string playerName = score.PlayerName.Split("#")[0];
                                playerName = playerName.Length > 12 ? playerName.Substring(0, 12) + ".." : playerName;

                                _aroundPlayerPositions[i].text = $"#{score.Rank + 1} {playerName} {formattedTime}";
                            }
                        }
                        return;
                    } 

                    // Fetch ranks around the player
                    var scoresAroundPlayer = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { Offset = Math.Max(0, playerRank - 2), Limit = 3 });

                    if (scoresAroundPlayer != null && scoresAroundPlayer.Results.Count > 0)
                    {
                        for (int i = 0; i < scoresAroundPlayer.Results.Count; i++)
                        {
                            var score = scoresAroundPlayer.Results[i];
                            TimeSpan time = TimeSpan.FromSeconds(score.Score);
                            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
                            string playerName = score.PlayerName.Split("#")[0];
                            playerName = playerName.Length > 12 ? playerName.Substring(0, 12) + ".." : playerName;

                            _aroundPlayerPositions[i].text = $"#{score.Rank + 1} {playerName} {formattedTime}";
                        }
                    }
                }

                return;
            }


            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { Offset = _topPositions.Length, Limit = _aroundPlayerPositions.Length });
            if (scoresResponse != null && scoresResponse.Results.Count > 0)
            {
                for (int i = 0; i < scoresResponse.Results.Count; i++)
                {
                    var score = scoresResponse.Results[i];
                    TimeSpan time = TimeSpan.FromSeconds(score.Score);
                    string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
                    string playerName = score.PlayerName.Split("#")[0];
                    playerName = playerName.Length > 12 ? playerName.Substring(0, 12) + ".." : playerName;

                    _aroundPlayerPositions[i].text = $"#{score.Rank + 1} {playerName} {formattedTime}";
                }
            }

            
        }
        catch (LeaderboardsException ex)
        {
            Debug.LogError($"Error fetching player scores: {ex.Message}");
        }
    }

    public async void AddScore(float score)
    {
        PlayerScoreData player = ScoreDataLoader.LoadPlayerScoreData();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(player.playerName);
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("TestLeaaderBoardd", score);
    }

    // Setup authentication event handlers if desired
    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () => {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    public async void GetPaginatedScores(int offset, int limit)
    {
        //var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("TestLeaaderBoardd",new GetScoresOptions { Offset = 0, Limit = 50 });
        //Debug.Log(JsonConvert.SerializeObject(scoresResponse));

        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("TestLeaaderBoardd", new GetScoresOptions { Offset = offset, Limit = limit });
            Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        }
        catch (LeaderboardsException ex)
        {
            Debug.LogError($"Error fetching paginated scores: {ex.Message}");
        }
    }
}
