using System;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;

public class MyNetworkManager : MonoBehaviour {

    #region SingletonPattern

    private static MyNetworkManager instance;

    public static MyNetworkManager Instance {
        get => instance;
    }

    public static event Action OnSingletonReady;

    private void Awake() {
        if (instance == null) {
            instance = this;
            OnSingletonReady?.Invoke();
        }
        else if (instance != this) {
            Debug.LogWarning("MyNetworkManager already exist.");
            Destroy(gameObject);
        }
    }

    #endregion
    
    public List<NetworkClient> clientlist;
    public static event Action OnClientListChange;
    // Start is called before the first frame update
    void Start() {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        UpdateClientList();
    }

    private void UpdateClientList() {
        clientlist = NetworkManager.Singleton.ConnectedClientsList;
        OnClientListChange?.Invoke();
    }

    private void OnServerStarted() {
        Debug.Log("Server Started");
    }

    private void OnClientConnected(ulong client_ID) {
        Debug.Log("Client Connected: " + client_ID);      
        UpdateClientList();

    }

    private void OnClientDisconnect(ulong client_ID) {
        Debug.Log("Client Disconnected: " + client_ID);
        if (NetworkManager.Singleton.IsHost) {
            return;
        }
        UpdateClientList();
    }
}