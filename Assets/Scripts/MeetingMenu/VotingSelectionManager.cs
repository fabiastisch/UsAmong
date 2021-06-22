using System;
using System.Collections;
using System.Collections.Generic;
using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
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

    public NetworkList<string> playerList = new NetworkList<string>(NetUtils.Everyone);

    public NetworkList<string> selectionList = new NetworkList<string>(new NetworkVariableSettings() {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<string>());

    public IEnumerable<string> GetPlayers() {
        return this.playerList;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerServerRPC() {
        Debug.Log("[VotingSelectionManager] SetPlayerServerRPC");
        playerList.Clear();
        var nameList =
            NetworkManager.Singleton.ConnectedClientsList.ConvertAll<string>(client =>
                client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value);
        foreach (string name in nameList) {
            playerList.Add(name);
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
        
        VotingSelection.Instance.ShowResultClientRpc(resultMessage);

        
        selectionList.Clear();
        
        AfterConsultationEvaluationClientRpc();

    }
    [ClientRpc]
    public void AfterConsultationEvaluationClientRpc() {
        CanvasLogic.Instance.StopCountdown();
    }

    public void ExecutePlayer(string electedToDie)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value == electedToDie[0].ToString())
            {
                client.PlayerObject.GetComponent<PlayerStuff>().DestroyMeServerRpc();
            }
        }
    }
}
     