using System;
using System.Collections.Generic;
using Lobby;
using MeetingMenu;
using MLAPI.Connection;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingSelection : MonoBehaviour {
    public GameObject newButtonPrefab;
    public GameObject parent;
    public TMP_Text canvasText;

    #region SingletonPattern

    private static VotingSelection instance;

    public static VotingSelection Instance {
        get => instance;
    }

    public static event Action OnSingletonReady;

    private void Awake() {
        if (instance == null) {
            instance = this;
            OnSingletonReady?.Invoke();
        }
        else if (instance != this) {
            Debug.LogWarning("ImposterSelection already exist.");
            Destroy(gameObject);
        }
    }

    #endregion

    private void OnEnable() {
        Debug.Log("[ImposterSelection] On Enable");
        PlayerSelectionUpdate();
    }


    public void PlayerSelectionUpdate() {
        Vector3 buttonPosition = parent.transform.position;

        for (int i = 0; i < parent.transform.childCount; i++) {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        
        // Store DeadPlayer to replace them to the End
        List<NetworkPlayerForMeeting> deadPlayer = new List<NetworkPlayerForMeeting>();

        foreach (NetworkPlayerForMeeting player in VotingSelectionManager.Instance.GetPlayers()) {
            if (!player.alive) {
                // Player is Dead
                deadPlayer.Add(player);
            }
            else {
                string playername = player.playerName;
                AddPlayerButton(buttonPosition, playername);
                // reicht für ca 4-5 Spieler
                buttonPosition.y -= 100;
            }
        }
        
        // Append DeadPlayer at the end
        foreach (NetworkPlayerForMeeting player in deadPlayer) {
            string playername = player.playerName;
            AddPlayerButton(buttonPosition, playername, false);
            // reicht für ca 4-5 Spieler
            buttonPosition.y -= 100;
        }
    }

    private void AddPlayerButton(Vector3 buttonPosition, string playername, bool enabled = true) {
        GameObject newButton = Instantiate(newButtonPrefab, buttonPosition, Quaternion.identity, parent.transform);
        if (newButton.transform.childCount < 1) {
            Debug.LogError("[VotingSelection]: Button ChildCount < 1. Should contain Text");
        }

        TMP_Text text = newButton.transform.GetChild(0).GetComponent<TMP_Text>();
        Button tempButton = newButton.GetComponent<Button>();
        tempButton.enabled = enabled;
        text.text = playername;
        tempButton.onClick.AddListener(() => MakeSelection(playername));
    }

    public void MakeSelection(string playerName) {
        Debug.Log("[ImposterSelection] MakeSelection");
        VotingSelectionManager.Instance.selectionList.Add(playerName);
    }

    public void ShowResultClient(string resultMessage) {
        canvasText.text = resultMessage;
        CanvasLogic.Instance.StopVoting();
    }
}