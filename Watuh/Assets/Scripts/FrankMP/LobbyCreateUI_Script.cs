using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class LobbyCreateUI_Script : MonoBehaviour
{
    VisualElement uiDoc;
    Button StartLobbyCreationButton;
    Button ChangePrivacyButton;
    Button ChangeDifficultyButton;
    string lobbyName;
    private Lobby joinedLobby;
    TextField LobbyNameField;
    TextField AmountOfPlayersField;
    Label WarningUI;
    Button WarningClose;
    string playerName;
    bool privacySetting = false;
    [SerializeField]
    private GameObject LobbyJoinedUI_GO;
    private float heartbeatTimer;
    private bool CanExecute = true;
    private float lobbyPollTimer;
    [SerializeField]
    private LobbyManagerUI lobbyManagerScript;
    [SerializeField]
    private ActiveLobbyScript activeLobbyScript;


    private void Init() 
    {
        uiDoc = this.gameObject.GetComponent<UIDocument>().rootVisualElement;
        ChangePrivacyButton = uiDoc.Query<Button>("ChangePrivacyButton");
        ChangePrivacyButton.clicked += ChangePrivacy;
        ChangeDifficultyButton = uiDoc.Query<Button>("ChangeDifficultyButton");
        ChangeDifficultyButton.clicked += ChangeDifficulty;
        LobbyNameField = uiDoc.Query<TextField>("LobbyNameField");
        AmountOfPlayersField = uiDoc.Query<TextField>("AmountOfPlayersField");
        StartLobbyCreationButton = uiDoc.Query<Button>("StartLobbyCreationButton");
        StartLobbyCreationButton.clicked += CreateLobby;
        WarningClose = uiDoc.Query<Button>("WarningCloseButton");
        WarningClose.clicked += CloseWarning;
        WarningUI = uiDoc.Query<Label>("WarningUI");
        playerName = PlayerPrefs.GetString("playerName");
    }

    private void CloseWarning()
    {
        WarningUI.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if(this.gameObject.GetComponent<UIDocument>().rootVisualElement != null && CanExecute == true)
        {
            Debug.Log(this.gameObject.GetComponent<UIDocument>().rootVisualElement);
            Init();
            CanExecute = false;
        }
        HandleLobbyHeartbeat();


    }
    private void ChangeDifficulty()
    {
        Debug.Log("Test12");
        string currentDiff = ChangeDifficultyButton.text;
            switch (currentDiff)
            {
                default:
                case "Easy":
                currentDiff = "Medium";
                    break;
                case "Medium":
                currentDiff = "Hard";
                    break;
            case "Hard":
                currentDiff = "Easy";
                break;
        }
        ChangeDifficultyButton.text = currentDiff;
            //UpdateLobbyGameMode(gameMode);
    }

    private void ChangePrivacy()
    {
        Debug.Log("Test1");
        string currentPriv = ChangePrivacyButton.text;
        switch (currentPriv)
        {
            default:
            case "Private":
                privacySetting = false;
                currentPriv = "Public";
                break;
            case "Public":
                privacySetting = true;
                currentPriv = "Private";
                break;
        }
        ChangePrivacyButton.text = currentPriv;
        //UpdateLobbyGameMode(gameMode);
    }

    private void DisplayWarning(string text)
    {
        WarningUI.style.display = DisplayStyle.Flex;
        WarningUI.text = text;
    }

    private async void CreateLobby()
    {
        int lobbyPlayers = Convert.ToInt32(AmountOfPlayersField.value);
        string lobbyName = LobbyNameField.value;
        bool hasFailed = false;
        if(lobbyName == "") { DisplayWarning("Lobby name can't be empty"); hasFailed = true; }
        if (lobbyPlayers > 5) { DisplayWarning("You can't have more than 5 players"); hasFailed = true; }
        if(hasFailed == false)
        {
            try
            {
                // querylobbyoptions for filters and stuff :D
                //string lobbyName = LobbyNameField.value;
                //int lobbyPlayers = Convert.ToInt32(AmountOfPlayersField.value);
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    Player = GetPlayer(),
                    IsPrivate = privacySetting,
                    Data = new Dictionary<string, DataObject>
               {
                   {"Difficulty", new DataObject(DataObject.VisibilityOptions.Public, "Easy", DataObject.IndexOptions.S1) },
                   {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, "0") }
               }
                };
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbyPlayers, createLobbyOptions);
                joinedLobby = lobby;
                lobbyManagerScript.joinedLobby = lobby;
                Debug.Log("Lobby create: " + lobby.Name);
                GetComponent<UIDocument>().enabled = false;
                LobbyJoinedUI_GO.gameObject.GetComponent<UIDocument>().enabled = true;
                Label LobbyCodeText = LobbyJoinedUI_GO.gameObject.GetComponent<UIDocument>().rootVisualElement.Query<Label>("LobbyCodeText");
                LobbyCodeText.text = lobby.LobbyCode;
                activeLobbyScript.thisLobby = lobby;
                activeLobbyScript.enabled = true;
                activeLobbyScript.thisLobby = lobby;
                activeLobbyScript.HandleLobbyPolling();

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }


    }



    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private void UpdateLobby()
    {

    }


}
