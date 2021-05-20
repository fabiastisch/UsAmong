using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Spawning;
using UnityEngine;

public class MyNetworkManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }


    private void OnServerStarted() {
        Debug.Log("Server Started");
    }

    private void OnClientConnected(ulong client_ID) {
        Debug.Log("Client Connected: " + client_ID);
    }

    private void OnClientDisconnect(ulong client_ID) {
        Debug.Log("Client Disconnected: " + client_ID);
        if (NetworkManager.Singleton.IsHost) {
            return;
        }
    }
}