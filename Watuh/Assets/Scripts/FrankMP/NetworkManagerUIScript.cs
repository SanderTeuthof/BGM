using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkManagerUIScript : MonoBehaviour
{

    private void Awake()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("StartHost").clicked += StartHost;
        root.Q<Button>("StartClient").clicked += StartClient;

    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        this.gameObject.SetActive(false);
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        this.gameObject.SetActive(false);
    }
}
