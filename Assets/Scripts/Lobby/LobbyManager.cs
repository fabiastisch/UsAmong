using System;
using System.Collections.Generic;
using System.Linq;
using Lobby;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Spawning;
using Player;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

// Script requires the GameObject to have a NetworkObject component
[RequireComponent(typeof(NetworkObject))]
public class LobbyManager : NetworkBehaviour {
    public GameObject playerObject;

    public GameObject deadPlayerObject;

    private GameObject player;

    public NetworkList<string> networkPlayerList = new NetworkList<string>(new NetworkVariableSettings() {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<string>());

    /**
     * Server only
     */
    public int impostersCount { get; private set; }

    /**
     * Event get's invoke, after Local Player was Spawned.
     */
    public static event Action<ulong> OnLocalPlayerSpawned;

    private static event Action<ulong> OnPlayerSpawned;

    public static event Action OnPlayerListUpdated;

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

    /**
     * On Server
     */
    private Vector3 startGamePos;


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

    /*
     * Server Side
     */
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
            Debug.Log("Local Player Obj: " + NetworkSpawnManager.GetLocalPlayerObject());
            OnLocalPlayerSpawned?.Invoke(clientId);
            Debug.Log("[SpawnedPlayerClientRpc]: " + clientId);
            UpdatePlayerListServerRPC();
        }

        OnPlayerSpawned?.Invoke(clientId);
    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerListServerRPC() {
        List<NetworkClient> list = MyNetworkManager.Instance.clientlist;
        List<string> playerList =
            list.ConvertAll<string>(client => client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value);

        Debug.Log("UpdatePlayerList");
        foreach (string playerName in playerList) {
            if (playerName.Equals("")) {
                Debug.LogWarning("Playername is empty");
                continue;
            }

            Debug.Log(playerName);
            if (!networkPlayerList.Contains(playerName)) {
                networkPlayerList.Add(playerName);
            }
        }

        OnPlayerListUpdated?.Invoke();
        UpdatePlayerListClientRpc();
    }

    [ClientRpc]
    private void UpdatePlayerListClientRpc() {
        OnPlayerListUpdated?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(Vector3 startGamePosPara) {
        
        this.startGamePos = startGamePosPara;
        int coundowntime = 3;
        PreStartGameClientRpc(coundowntime);
        SetImposters();
        Debug.Log("[AfterImpSelection]");
        Invoke(nameof(StartGame), coundowntime);
    }

    /**
     * Called on Server
     */
    public void SetImposters() {
        NetworkClient[] networkClients = new NetworkClient[NetworkManager.Singleton.ConnectedClientsList.Count];
        NetworkManager.Singleton.ConnectedClientsList.CopyTo(networkClients);
        List<NetworkClient> playerList = networkClients.ToList();
        

        var imposters = new List<NetworkClient>();
        if (playerList.Count > 3) {
            // 2 Imposter
            for (int i = 0; i < 2; i++) {
                int random = UtilsUnity.GetRandomInt(playerList.Count - 1);
                imposters.Add(playerList[random]);
                playerList.RemoveAt(random);
            }
            
        }else if (playerList.Count > 1) {
            // 1 Imposter
            int random = UtilsUnity.GetRandomInt(playerList.Count - 1);
            imposters.Add(playerList[random]);
            playerList.RemoveAt(random);
            //playerList.Remove(playerList[random]);
        }

        this.impostersCount = imposters.Count;
        

        Debug.Log("[SetImposters]: imposter: " +imposters.Count + "imposter: " + imposters[0].PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value);
        Debug.Log("[SetImposters]: playerlist: " +playerList.Count);
        
        foreach (NetworkClient imposter in imposters) {
            if (imposter.PlayerObject) {
                PlayerLife playerLife = imposter.PlayerObject.GetComponent<PlayerLife>();
                playerLife.isImposterNetVar.Value = true;
            }
            else {
                Debug.LogError("[LobbyManager:SetImposters]: no PlayerObject");
            }
        }
    }

    /**
     * On Server to call ClientRpc
     */
    private void StartGame() {
        Debug.Log("[StartGame]");
        StartGameClientRpc(startGamePos);
    }

    [ClientRpc]
    private void PreStartGameClientRpc(int coundowntime) {
        CanvasLogic.Instance.StartCountdown(coundowntime);
    }

    [ClientRpc]
    public void StartGameClientRpc(Vector3 startGamePosition) {
        Debug.Log("[StartGameClientRpc]");
        CanvasLogic.Instance.StopCountdown();
        GameObject localPlayer = this.getLocalPlayer();
        Debug.Log("Got local Player: " + localPlayer);
        Debug.Log("move player: " + localPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
        localPlayer.transform.position = startGamePosition;
        // startGamePos += new Vector3(2f, 0);
    }


    [ServerRpc(RequireOwnership = false)]
    public void DestroyMeServerRpc() {
        player.GetComponent<NetworkObject>().Despawn(true);
    }

    public GameObject getLocalPlayer() {
        return NetworkSpawnManager.GetLocalPlayerObject().gameObject;
    }
}