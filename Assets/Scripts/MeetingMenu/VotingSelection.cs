using System;
using System.Collections;
using System.Collections.Generic;
using Lobby;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VotingSelection : MonoBehaviour {
    public GameObject newButtonPrefab;
    public GameObject parent;
    public TMP_Text canvasText;

    public NetworkList<string> selectionList = new NetworkList<string>(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<string>());
    
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

        foreach (string player in VotingSelectionManager.Instance.GetPlayers()) {
            GameObject newButton = Instantiate(newButtonPrefab, buttonPosition, Quaternion.identity,parent.transform);
            //newButton.transform.position = position;
            //newButton.transform.SetParent(parent.transform);

            //tempButton.name = player;
            if (newButton.transform.childCount < 1) {
                Debug.LogError("[ImposterSelection]: Button ChildCount < 1. Should contain Text");
            }
            TMP_Text text = newButton.transform.GetChild(0).GetComponent<TMP_Text>();
            text.text = player;
            Button tempButton = newButton.GetComponent<Button>();
            tempButton.onClick.AddListener(() => MakeSelection(player));

            // reicht für ca 4-5 Spieler
            buttonPosition.y -= 100;
        }
    }


    public void MakeSelection(string playerName) {
        Debug.Log("[ImposterSelection] MakeSelection");
        VotingSelectionManager.Instance.selectionList.Add(playerName);
    }

    [ClientRpc]
    public void ShowResultClientRpc(string resultMessage)
    {
        canvasText.text = resultMessage;
        CanvasLogic.Instance.StopVoting();
        CanvasLogic.Instance.StartShowingResult();
        Invoke(nameof(InvokeStopShowingResult), 4);
    }

    public void InvokeStopShowingResult()
    {
        CanvasLogic.Instance.StopShowingResult();
    }


    
    
    
  
}