using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

// Script requires the GameObject to have a NetworkObject component
[RequireComponent(typeof(NetworkObject))]
public class LobbyManager : NetworkBehaviour {
    public GameObject playerObject;

    private GameObject player;
    
    public static event Action<ulong> OnPlayerSpawned;

    #region SingletonPattern

    public static LobbyManager Singleton { get; private set; }

    public static event Action OnSingletonReady;

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
            OnSingletonReady?.Invoke();
        }
        else if (Singleton != this) {
            Debug.LogWarning("Lobby Manager already exist.");
            Destroy(this);
        }
    }

    #endregion
    
    // Start is called before the first frame update
    void Start() {
        if (!NetUtils.IsConnected()) {
            return;
        }

        if (NetUtils.IsServer()) {
            Debug.Log("Server Spawn");
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
        else {
            Debug.Log("Invoke RPC");

            SpawnMeServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnMeServerRpc(ulong clientId) {
        Debug.Log("Server RPC"); 
        SpawnPlayer(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyMeServerRpc() {
        player.GetComponent<NetworkObject>().Despawn(true);
    }

    private void SpawnPlayer(ulong clientId) {
        Vector3 random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        player = Instantiate(playerObject, random, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
        SpawnedPlayerClientRpc(clientId);
    }

    [ClientRpc]
    private void SpawnedPlayerClientRpc(ulong clientId) {
        if (clientId.Equals(NetUtils.LocalClientId)) {
            OnPlayerSpawned?.Invoke(clientId);
        } 
    }
}