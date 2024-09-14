using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System;
using Unity.Services.Core;
using System.Linq;

public class LobbyManagerUI : MonoBehaviour
{
    public Lobby joinedLobby;
    VisualElement uiDoc;
    Button CreateLobbyButton;
    Button JoinLobbyByCodeButton;
    string playerName;
    [SerializeField]
    private GameObject LobbyCreateUI_GO;
    [SerializeField]
    private GameObject JoinLobbyByCodeUI_GO;
    [SerializeField]
    private GameObject JoinedLobbyUI_GO;
    VisualElement LobbyListView;
    private float refreshLobbyListTimer = 5f;

    [SerializeField]
    VisualTreeAsset LobbyListTemp;

    Dictionary<string,Button> lobbiesInList = new Dictionary<string,Button>();

    bool hasInitzialied = false;

    private void Init()
    {
        uiDoc = this.gameObject.GetComponent<UIDocument>().rootVisualElement;
        CreateLobbyButton = uiDoc.Query<Button>("CreateLobbyButton");
        CreateLobbyButton.clicked += CreateLobbyUI;
        JoinLobbyByCodeButton = uiDoc.Query<Button>("JoinLobbyWithCodeButton");
        //JoinLobbyByCodeButton.clicked += JoinLobbyByCodeUI;
        playerName = PlayerPrefs.GetString("playerName");
        LobbyListView = uiDoc.Query<VisualElement>("LobbyListView");
        ListLobbies();
    }

    private void Update()
    {
        if(this.gameObject.GetComponent<UIDocument>().rootVisualElement != null && hasInitzialied == false && AuthenticationService.Instance.IsAuthorized == true)
        {
            Debug.Log("Quik test: " + AuthenticationService.Instance.PlayerId);
            Init();
            hasInitzialied =true;
        }
        if(joinedLobby == null && AuthenticationService.Instance.IsAuthorized == true)
        {
            HandleRefreshLobbyList();
        }
       // JoinedLobbyUI_GO.gameObject.GetComponent<ActiveLobbyScript>().HandleLobbyPolling();
    }

    private void JoinLobbyByCodeUI()
    {
        GetComponent<UIDocument>().enabled = false;
        JoinLobbyByCodeUI_GO.GetComponent<UIDocument>().enabled = true;
    }

    private void CreateLobbyUI()
    {
        this.gameObject.GetComponent<UIDocument>().enabled = false;
        LobbyCreateUI_GO.gameObject.GetComponent<UIDocument>().enabled = true;
    }

    private void GoIntoLobbyUI()
    {
        this.gameObject.GetComponent<UIDocument>().enabled = false;
        JoinedLobbyUI_GO.gameObject.GetComponent<UIDocument>().enabled = true;
        JoinedLobbyUI_GO.gameObject.GetComponent<ActiveLobbyScript>().thisLobby = joinedLobby;
        JoinedLobbyUI_GO.gameObject.GetComponent<ActiveLobbyScript>().enabled = true;
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {

        try
        {
            Player player = GetPlayer();

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
            {
                Player = player
            });

            joinedLobby = lobby;
            GoIntoLobbyUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
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

    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 15f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                ListLobbies();
            }
        }
    }

    private async void ListLobbies()
    {
//        Debug.Log(AuthenticationService.Instance.IsAuthorized);
  //      Debug.Log("Quik test: " + AuthenticationService.Instance.PlayerId);
        try
        {
            lobbiesInList.Clear();
            LobbyListView.hierarchy.Clear();

            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };


            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies found: " + lobbyListQueryResponse.Results.Count);
            foreach (Lobby lobby in lobbyListQueryResponse.Results)
            {
                VisualElement newLobbyEntryUI = LobbyListTemp.Instantiate();

                Label LobbyNameLabel = newLobbyEntryUI.Query<Label>("LobbyName");
                Label PlayersInLobbyLabel = newLobbyEntryUI.Query<Label>("PlayersInLobby");
                Label DifficultyLabel = newLobbyEntryUI.Query<Label>("Difficulty");
                Button joinLobbyButton = newLobbyEntryUI.Query<Button>("JoinLobbyButton");

                joinLobbyButton.clicked += () => LobbyJoinCLicked(joinLobbyButton);
                lobbiesInList.Add(lobby.Id, joinLobbyButton);

                LobbyNameLabel.text = lobby.Name;
                PlayersInLobbyLabel.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
                DifficultyLabel.text = lobby.Data["Difficulty"].Value;

                LobbyListView.hierarchy.Add(newLobbyEntryUI);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Found error!!!");
            Debug.Log(e);
        }
    }

    private void LobbyJoinCLicked(Button button)
    {
        string lobbyId = lobbiesInList.FirstOrDefault(x => x.Value == button).Key;
        JoinLobbyById(lobbyId);
    }

    public async void JoinLobbyById(string lobbyId)
    {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions
        {
            Player = player
        });
        GoIntoLobbyUI();
    }

}
