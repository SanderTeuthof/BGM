using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float time = 0;
    private float lobbyUpdateTimer = 0;
    string playerName;
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateLobby()
    {
        try
        {
            // querylobbyoptions for filters and stuff :D
            string lobbyName = "My super cool lobby";
            int lobbyPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
               Data  = new Dictionary<string, DataObject>
               {
                   {"Difficulty", new DataObject(DataObject.VisibilityOptions.Public, "Easy", DataObject.IndexOptions.S1) }
               }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbyPlayers);
            hostLobby = lobby;
            Debug.Log("Lobby create: " + lobby.Name + " " + lobby.Players);
        } 
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse response =  await Lobbies.Instance.QueryLobbiesAsync();
        Debug.Log("Lobbies found: " + response.Results.Count);
        foreach(Lobby lobby in response.Results)
        {
            Debug.Log(lobby.Name + "One of the lobbies");
        }
    } 
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        LobbyHeartbeat();
        HandleLobbyUpdate();
    }

    private async void UpdateLobbyDifficulty(string gamemode, Lobby lobby)
    {
        try
        {
            lobby = await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                { "Difficulty", new DataObject(DataObject.VisibilityOptions.Public, "Hard") }
            }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Test2") }
            }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void KickPlayer(string id)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void HandleLobbyUpdate()
    {
        if (hostLobby != null)
        {
            lobbyUpdateTimer++;
            if (lobbyUpdateTimer >= 15)
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
                lobbyUpdateTimer = 0;
            }
        }
    }


    private async void LobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            time++;
            if (time >= 15)
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                time = 0;
            }
        }
    }

    private async void joinLobby()
    {
        try
        {


            await Lobbies.Instance.JoinLobbyByIdAsync("1");
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

    private async void joinLobbyByCode(string code)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };
            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);
            GetPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void QuikJoin()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void GetPlayers(Lobby lobby)
    {
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
