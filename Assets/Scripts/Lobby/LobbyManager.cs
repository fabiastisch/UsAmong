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
using Teleport;
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

    public NetworkVariableInt livingCrewMates = new NetworkVariableInt(NetUtils.Everyone, 0);


    /**
     * Current ImposterCount
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

    /**
     * On Server
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

        foreach (string playerName in networkPlayerList) {
            // Remove Disconnected Player
            if (!playerList.Contains(playerName)) {
                networkPlayerList.Remove(playerName);
            }
        }

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
    public void StartGameServerRpc() {
        int coundowntime = 3;
        PreStartGameClientRpc(coundowntime);
        SetImposters();
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
        if (playerList.Count > 5) {
            // 2 Imposter
            for (int i = 0; i < 2; i++) {
                int random = UtilsUnity.GetRandomInt(playerList.Count - 1);
                imposters.Add(playerList[random]);
                playerList.RemoveAt(random);
            }
        }
        else if (playerList.Count > 1) {
            // 1 Imposter
            int random = UtilsUnity.GetRandomInt(playerList.Count - 1);
            imposters.Add(playerList[random]);
            playerList.RemoveAt(random);
            //playerList.Remove(playerList[random]);
        }

        this.impostersCount = imposters.Count;


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
     * Server only
     */
    public void MinimizeImposterNumber() {
        this.impostersCount -= 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetermineNumberOfLivingCrewmatesServerRPC() {
        Debug.Log("[LobbyManager] DetermineNumberOfLivingCrewmatesServerRPC");
        DetermineNumberOfLivingCrewmatesOnServer();
    }

    /**
     * On Server
     */
    public void DetermineNumberOfLivingCrewmatesOnServer() {
        livingCrewMates.Value = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList) {
            PlayerLife playerLife = client.PlayerObject.GetComponent<PlayerLife>();
            if (playerLife.isAliveNetVar.Value && !playerLife.isImposterNetVar.Value) {
                // if Alive & not Imposter
                livingCrewMates.Value++;
            }
        }
    }

    /**
     * On Server to call ClientRpc
     */
    private void StartGame() {
        Debug.Log("[LobbyManager][AfterImpSelection]:StartGame");
        StartGameClientRpc();
    }

    [ClientRpc]
    private void PreStartGameClientRpc(int coundowntime) {
        CanvasLogic.Instance.StartCountdown(coundowntime);
    }

    [ClientRpc]
    public void StartGameClientRpc() {
        Debug.Log("[StartGameClientRpc]");
        CanvasLogic.Instance.StopCountdown();
        CanvasLogic.Instance.SetYoureImpOrCrewMate(2);
        GameObject localPlayer = this.getLocalPlayer();

        Vector3 random = new Vector3(Random.Range(-65f, -55f), Random.Range(-75f, -85f), 0);
        localPlayer.transform.position = random;
    }


    [ServerRpc(RequireOwnership = false)]
    public void ResetGameServerRpc() {
        ResetGameClientRpc();
    }

    /**
     *  On Server
     */
    public void ResetGameOnServer() {
        ResetGameClientRpc();
    }


    [ClientRpc]
    public void ResetGameClientRpc() {
        GameObject playerObj = getLocalPlayer();

        NetworkVariableString playerName = playerObj.GetComponent<PlayerStuff>().PlayerName;
        
        PlayerLife playerLife = playerObj.GetComponent<PlayerLife>();
        playerLife.DestroyDeadBodyServerRPC();
        playerLife.isImposterNetVar.Value = false;
        playerLife.isAliveNetVar.Value = true;

        PlayerControls playerControls = playerObj.GetComponent<PlayerControls>();
        playerControls.coolDownTime = 0f;
        playerControls.killCoolDownActive = false;

        CoinManager.Instance.DestroyAllLocalCoins();
        CoinManager.Instance.remainingCoinsNetVar.Value = 0;

        Vector3 random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        TeleportManager.Instance.TeleportationServerRpc(random);

        CanvasLogic.Instance.inGame = false;
    }


    public GameObject getLocalPlayer() {
        return NetworkSpawnManager.GetLocalPlayerObject().gameObject;
    }
}