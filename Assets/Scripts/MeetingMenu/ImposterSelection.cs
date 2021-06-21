using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImposterSelection : MonoBehaviour {
    public GameObject newButtonPrefab;
    public GameObject parent;

    public NetworkList<string> selectionList = new NetworkList<string>(new NetworkVariableSettings()
    {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<string>());
    
    #region SingletonPattern

    private static ImposterSelection instance;

    public static ImposterSelection Instance {
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

    // Start is called before the first frame update
    void Start() {
       
    }
    
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
            Debug.Log("Parent Trans: " + newButton.transform.parent.transform.position);
            //newButton.transform.position = position;
            //newButton.transform.SetParent(parent.transform);

            //tempButton.name = player;
            if (newButton.transform.childCount < 1) {
                Debug.LogError("[ImposterSelection]: Button ChildCount < 1. Should contain Text");
            }
            TMP_Text text = newButton.transform.GetChild(0).GetComponent<TMP_Text>();
            Debug.Log(text);
            text.text = player;
            Button tempButton = newButton.GetComponent<Button>();
            tempButton.onClick.AddListener(() => MakeSelection(player));

            // reicht für ca 4-5 Spieler
            buttonPosition.y += 100;
        }
    }


    public void MakeSelection(string playerName) {
        Debug.Log("[ImposterSelection] MakeSelection");
        VotingSelectionManager.Instance.selectionList.Add(playerName);
    }


    

    public void DetermineResult(ArrayList deathList) {
        Text text = parent.AddComponent<Text>();
        switch (deathList.Count) {
            case 0:
                Debug.Log("Kein Spieler wurde gewählt");
                text.text = "Kein Spieler wurde gewählt";
                break;
            case 1:
                Debug.Log("Ein Spieler wurde gewählt");
                //ExecutePlayer(deathList[0].ToString());
                break;
            default:
                Debug.Log("Es wurde keine Einigung getroffen. Mehrere Spieler haben die gleiche Punktzahl");
                break;
        }
        /*
        MainMenu.ActivateImposterSelection();
        */
    }

    private void ExecutePlayer(string playerName) {
        GameObject player = GameObject.Find(playerName);
        player.GetComponent<PlayerStuff>().DestroyMeServerRpc();
    }
}