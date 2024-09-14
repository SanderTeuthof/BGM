using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.InputSystem;

public class Leaderboarding : MonoBehaviour
{

    [SerializeField]
    private float scoreChange;

    //public void Get
    private async void Awake()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile("banana");

        await UnityServices.InitializeAsync(options);


        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void AddScore()
    {
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("TestLeaaderBoardd", 102);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
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

    // Start is called before the first frame update
    void Start()
    {
        SetupEvents();
     //   AddScore();
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyB = Keyboard.current;
        if(keyB.eKey.isPressed )
        {
            GetPaginatedScores();
        }
        if (keyB.qKey.isPressed)
        {
            AddScore();
        }
    }

    public async void GetPaginatedScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("TestLeaaderBoardd",new GetScoresOptions { Offset = 0, Limit = 50 });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}
