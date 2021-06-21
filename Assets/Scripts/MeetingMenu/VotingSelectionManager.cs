using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Debug.LogWarning("VotingSelectionManager already exist.");
            Destroy(gameObject);
        }
    }

    #endregion

    public NetworkList<string> playerList = new NetworkList<string>(NetUtils.Everyone);

    public NetworkList<string> selectionList = new NetworkList<string>(new NetworkVariableSettings() {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<string>());

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

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

        /*Dictionary<string, int> selectionResult = new Dictionary<string, int>();


        foreach (string selectedPlayer in selectionList) {
            if (!selectionResult.ContainsKey(selectedPlayer)) {
                selectionResult.Add(selectedPlayer, 1);
            }
            else {
                selectionResult[selectedPlayer] += 1;
            }
        }*/


        var result = selectionList.GroupBy(x => x)
            .Select(g => new {Value = g.Key, Count = g.Count()})
            .OrderByDescending(x => x.Count);

        foreach (var val in result) {
            Debug.Log("[VotingSelectionManager]: " + val.Count + " on Player: " + val.Value);
        }


        selectionList.Clear();

        //EveluateConsultationClientRpc(selectionResult);
    }


    /*
    [ClientRpc]
    public void EveluateConsultationClientRpc(Dictionary<string, int> selectIonResult) {
        ArrayList electedToDie = new ArrayList();
        foreach (var player in selectIonResult.Keys) {
            Debug.Log(player + ": " + selectIonResult[player]);

            if (electedToDie.Count != 0 && selectIonResult[player] > selectIonResult[electedToDie[0].ToString()]) {
                electedToDie = new ArrayList();
                electedToDie.Add(player);
            }
            else if (electedToDie.Count != 0 && selectIonResult[player] == selectIonResult[electedToDie[0].ToString()]) {
                electedToDie.Add(player);
            }
        }

        ImposterSelection.Instance.DetermineResult(electedToDie);
    }*/
}