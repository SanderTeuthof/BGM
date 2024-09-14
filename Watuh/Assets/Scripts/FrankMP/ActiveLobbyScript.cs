using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine.SceneManagement;

public class ActiveLobbyScript : MonoBehaviour
{
    public Lobby thisLobby;
    VisualElement PlayerListView;
    Button StartGameButton;
    [SerializeField]
    VisualTreeAsset PlayerListTemp;
    private float lobbyPollTimer;
    private bool isRelay = false;
    string relayCode = "0";

    private void Start()
    {
        PlayerListView = this.gameObject.GetComponent<UIDocument>().rootVisualElement.Query<VisualElement>("PlayerList");
        StartGameButton = this.gameObject.GetComponent<UIDocument>().rootVisualElement.Query<Button>("StartGameButton");
        StartGameButton.clicked += StartRelay;
        foreach (Player player in thisLobby.Players)
        {
            VisualElement newLobbyEntryUI = PlayerListTemp.Instantiate();
            Label playerNameLabel = newLobbyEntryUI.Query<Label>("PlayerNameLabel");
            playerNameLabel.text = player.Data["PlayerName"].Value;
            PlayerListView.Add(newLobbyEntryUI);

        }
    }

    private async void StartRelay()
    {
        Debug.Log("RelayStarting");
        try
        {
            if (IsLobbyHost())
              {
                    Allocation allocation = await RelayService.Instance.CreateAllocationAsync(thisLobby.MaxPlayers);

                    string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(thisLobby.Id, new UpdateLobbyOptions
                { Data = new Dictionary<string, DataObject> {
                    {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
                });

                    RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                    NetworkManager.Singleton.StartHost();
                //relayCode = joinCode;
                //SceneManager.LoadScene("GameScene");
                //Destroy(this);
                this.gameObject.GetComponent<UIDocument>().enabled = false;
                this.enabled = false;
            }
            }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async void StartJoiningRelay()
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            //SceneManager.LoadScene("GameScene");
            this.gameObject.GetComponent<UIDocument>().enabled = false;
            this.enabled = false;
            //Destroy(this);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
    private void Update()
    {
        if(thisLobby != null)
        {
            HandleLobbyPolling();
        }
        if (relayCode != "0" && IsLobbyHost() == false && NetworkManager.Singleton.IsClient == false)
        {
            relayCode = thisLobby.Data["RelayCode"].Value;
            StartJoiningRelay();
        }
    }

     public async void HandleLobbyPolling()
     {
         if (thisLobby != null && relayCode == "0")
         {
            Debug.Log("Test1");
             lobbyPollTimer -= Time.deltaTime;
             if (lobbyPollTimer < 0f)
             {
                 float lobbyPollTimerMax = 1.1f;
                 lobbyPollTimer = lobbyPollTimerMax;
                Debug.Log("Test2");
                thisLobby = await LobbyService.Instance.GetLobbyAsync(thisLobby.Id);
                if (thisLobby.Data["RelayCode"].Value != "0")
                {
                    Debug.Log("Test3");
                    relayCode = thisLobby.Data["RelayCode"].Value;
                }
                Debug.Log("Test4");
                UpdateLobby();
/*
             if (!IsPlayerInLobby())
             {
                 // Player was kicked out of this lobby
                 Debug.Log("Kicked from Lobby!");

                 OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                 joinedLobby = null;
             }
                */
             }
         }
     }


    public void UpdateLobby()
    {
        Debug.Log("Test5");
        if (this != null)
        {
            Debug.Log("Test6");
            PlayerListView = gameObject.GetComponent<UIDocument>().rootVisualElement.Query<VisualElement>("PlayerList");
            PlayerListView.Clear();
            Debug.Log("Test7");
            foreach (Player player in thisLobby.Players)
            {
                Debug.Log("Test8");
                VisualElement newLobbyEntryUI = PlayerListTemp.Instantiate();
                Label playerNameLabel = newLobbyEntryUI.Query<Label>("PlayerNameLabel");
                playerNameLabel.text = player.Data["PlayerName"].Value;
                PlayerListView.Add(newLobbyEntryUI);

            }
        }
    }

    public bool IsLobbyHost()
    {
        return thisLobby != null && thisLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
}
