using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Spawning;
using UnityEngine;

public class MyNetworkManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    /**
         * Happen on server
         */
    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback){
        Debug.Log("Approving a connection");
        // logic
        bool approve = true;
        bool createPlayerObject = false;
        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
        ulong? prefabHash = NetworkSpawnManager.GetPrefabHashFromGenerator("MyPrefabHashGenerator");
    
        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        callback(createPlayerObject, null, approve, null, null);
    }
    private void OnServerStarted()
    {
        Debug.Log("Server Started");
    }
    private void OnClientConnected(ulong client_ID)
    {
        Debug.Log("Client Connected");
    }
    private void OnClientDisconnect(ulong client_ID)
    {
        Debug.Log("Client Disconnected");
        if (NetworkManager.Singleton.IsHost) { return; }
            
    }
}