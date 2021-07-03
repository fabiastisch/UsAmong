using System;
using System.Collections;
using System.Collections.Generic;
using Lobby;
using MeetingMenu;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Logging;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
using Teleport;
using UnityEngine;
using Utils;

[RequireComponent(typeof(NetworkObject))]
public class VotingSelectionManager : NetworkBehaviour {
    #region SingletonPattern

    private static VotingSelectionManager instance;

    public static VotingSelectionManager Instance {
        get => instance;
    }

    public static event Action OnSingletonReady;

    private void Awake() {
        if (instance == null) {
            instance = this;
            OnSingletonReady?.Invoke();
        }
        else if (instance != this) {
            instance = this;
            Debug.LogWarning("VotingSelectionManager already exist.");
            //Destroy(gameObject);
        }
    }

    #endregion

    public NetworkList<NetworkPlayerForMeeting> playerList = new NetworkList<NetworkPlayerForMeeting>(NetUtils.Everyone);

    public NetworkList<string> selectionList = new NetworkList<string>(new NetworkVariableSettings() {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<string>());

    public IEnumerable<NetworkPlayerForMeeting> GetPlayers() {
        return this.playerList;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerServerRPC() {
        Debug.Log("[VotingSelectionManager] SetPlayerServerRPC");
        playerList.Clear();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList) {
            string playerName = client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value;
            bool alive = client.PlayerObject.GetComponent<PlayerLife>().isAliveNetVar.Value;
            playerList.Add(new NetworkPlayerForMeeting(client.ClientId,playerName, alive));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EveluateConsultationServerRpc() {
        Debug.Log("[VotingSelectionManager] EveluateConsultationServerRpc");

        Dictionary<string, int> selectionResult = new Dictionary<string, int>();


        foreach (string selectedPlayer in selectionList) {
            if (!selectionResult.ContainsKey(selectedPlayer)) {
                selectionResult.Add(selectedPlayer, 1);
            }
            else {
                selectionResult[selectedPlayer] += 1;
            }
        }

        ArrayList electedToDie = new ArrayList();
        foreach (var player in selectionResult.Keys) {
            Debug.Log(player + ": " + selectionResult[player]);

            if (electedToDie.Count == 0) {
                electedToDie.Add(player);
            }
            else if (selectionResult[player] > selectionResult[electedToDie[0].ToString()]) {
                electedToDie = new ArrayList();
                electedToDie.Add(player);
            }
            else if (selectionResult[player] == selectionResult[electedToDie[0].ToString()]) {
                electedToDie.Add(player);
            }
        }


        String resultMessage;

        if (electedToDie.Count == 1) {
            Debug.Log("[VotingSelectionManager] : " + electedToDie[0].ToString() + " wurde rausgevotet.");
            ExecutePlayer(electedToDie[0].ToString());
            resultMessage = electedToDie[0].ToString() + " wurde rausgevotet.";
        }
        else {
            Debug.Log("[VotingSelectionManager] : Niemand wurde rausgevotet.");
            resultMessage = "Keine eindeutige Entscheidung ";
        }

        selectionList.Clear();
        
        DetermineVictory();

        if (LobbyManager.Singleton.inGameNetworkVar.Value) {
            Invoke(nameof(TeleportPlayerAfterVoting), 4);
            ShowResultClientRpc(resultMessage);
        }
    }
    
    /**
         * Determines victory by existing Crewmates or Imposter
         * 
         * On Server
         */
    public bool DetermineVictory() {
        LobbyManager lobbyManager = LobbyManager.Singleton;
        
        if (lobbyManager.livingCrewMates.Value <= lobbyManager.impostersCount) {
            SetWinScreenClientRPC(true);
            LobbyManager.Singleton.ResetGameOnServer();
            return true;
        }
        else if (lobbyManager.impostersCount == 0) {
            SetWinScreenClientRPC(false);
            LobbyManager.Singleton.ResetGameOnServer();
            return true;
        }

        return false;
    }

    [ClientRpc]
    private void SetWinScreenClientRPC(bool isImposterWinScreen) {
        if (isImposterWinScreen) {
            CanvasLogic.Instance.StartWinScreen(true);
        }
        else {
            CanvasLogic.Instance.StartWinScreen(false);
        }
    }

    private void TeleportPlayerAfterVoting() {
        TeleportManager.Instance.TeleportationClientRpc(new Vector3(-60,-80));
    }

    [ClientRpc]
    public void ShowResultClientRpc(string resultMessage) {
        VotingSelection.Instance.ShowResultClient(resultMessage);
    }

    public void ExecutePlayer(string electedToDie) {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList) {
            if (client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value.Equals(electedToDie)) {
                client.PlayerObject.GetComponent<PlayerLife>().Kill();
                NetworkLog.LogWarningServer("[Execute]:Equals");
            }
        }
    }
}