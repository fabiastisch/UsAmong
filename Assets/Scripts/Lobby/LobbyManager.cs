using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.Spawning;
using Player;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

// Script requires the GameObject to have a NetworkObject component
[RequireComponent(typeof(NetworkObject))]
public class LobbyManager : NetworkBehaviour {
    public GameObject playerObject;

    private GameObject player;

    /**
     * Event get's invoke, after Local Player was Spawned.
     */
    public static event Action<ulong> OnLocalPlayerSpawned;

    private static event Action<ulong> OnPlayerSpawned;

    public static event Action<string[]> OnPlayerListUpdated;

    #region SingletonPattern

    public static LobbyManager Singleton { get; private set; }

    public static event Action OnSingletonReady;

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
            OnSingletonReady?.Invoke();
            OnPlayerSpawned += this.OnNewPlayerSpawned;
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

    private void SpawnPlayer(ulong clientId) {
        Vector3 random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        player = Instantiate(playerObject, random, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
        SpawnedPlayerClientRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnMeServerRpc(ulong clientId) {
        Debug.Log("Server RPC");
        SpawnPlayer(clientId);
    }

    [ClientRpc]
    private void SpawnedPlayerClientRpc(ulong clientId) {
        if (clientId.Equals(NetUtils.LocalClientId)) {
            OnLocalPlayerSpawned?.Invoke(clientId);
            UpdatePlayerListServerRpc(clientId);
        }

        OnPlayerSpawned?.Invoke(clientId);
    }

    private void OnNewPlayerSpawned(ulong clientId) {
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerListServerRpc(ulong clientId) {
        List<NetworkClient> list = MyNetworkManager.Instance.clientlist;
        List<string> playerNameList =
            list.ConvertAll<string>(client => client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value);
        UpdatePlayerListClientRpc(playerNameList.ToArray());
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(Vector3 startGamePos) {
        StartGameClientRpc(startGamePos);
    }

    [ClientRpc]
    public void StartGameClientRpc(Vector3 startGamePos) {
        GameObject localPlayer = this.getLocalPlayer();
        Debug.Log("move player: " + localPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
        localPlayer.transform.position = startGamePos;
        // startGamePos += new Vector3(2f, 0);
    }
    
    [ClientRpc]
    private void UpdatePlayerListClientRpc(string[] playerNameList) {
        //Debug.Log("UpdatePlayerListClientRpc"+playerNameList.Length);
        OnPlayerListUpdated?.Invoke(playerNameList);
    }


    [ServerRpc(RequireOwnership = false)]
    public void DestroyMeServerRpc() {
        player.GetComponent<NetworkObject>().Despawn(true);
    }

    public GameObject getLocalPlayer() {
        return NetworkSpawnManager.GetLocalPlayerObject().gameObject;;
    }
}