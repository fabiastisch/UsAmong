using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class ImposterSelection : MonoBehaviour
{
    
    public GameObject newButtonPrefab;
    public GameObject parent;
    private ArrayList players = new ArrayList();

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
            Debug.LogWarning("TabMenu already exist.");
            Destroy(gameObject);
        }
    }

    #endregion

    private ArrayList playerList;
    // Start is called before the first frame update
    void Start()
    {
        LobbyManager.OnSingletonReady += () =>
            LobbyManager.Singleton.networkPlayerList.OnListChanged += NetworkPlayerListOnOnListChanged;
        
    }

    private void NetworkPlayerListOnOnListChanged(NetworkListEvent<string> changeevent)
    {
        foreach (string playerName in LobbyManager.Singleton.networkPlayerList)
        {
            players.Add(playerName);
        }

        PlayerSelectionUpdate();
    }


    public void PlayerSelectionUpdate()
    {
        Vector3 position = new Vector3(-400, 250, 0);

        foreach (string player in players){
        
            GameObject newButton = Instantiate(newButtonPrefab); 
            newButton.transform.SetParent(parent.transform, false);
            newButton.transform.localScale = position;
            Button tempButton = newButton.GetComponent<Button>();
            tempButton.name = player;
            Text text = tempButton.GetComponent<Text>();
            text.text = player;
            tempButton.onClick.AddListener(() => MakeSelection(player));
            
            // reicht für ca 4-5 Spieler
            position.y += 100;
        }
    }


    public void MakeSelection(string playerName)
    {
        selectionList.Add(playerName);
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void EveluateConsultationServerRpc()
    {
        Dictionary<string, int> selectionResult = new Dictionary<string, int>();
        
        foreach (string selectedPlayer in selectionList)
        {
            if (!selectionResult.ContainsKey(selectedPlayer))
            {
                selectionResult.Add(selectedPlayer, 1);
            }
            else
            {
                selectionResult[selectedPlayer] += 1;
            }
        }

        EveluateConsultationClientRpc(selectionResult);
    }


    [ClientRpc]
    public void EveluateConsultationClientRpc(Dictionary<string, int> selectIonResult)
    {

        ArrayList electedToDie = new ArrayList();
        foreach (var player in selectIonResult.Keys)
        {
            Debug.Log(player + ": " + selectIonResult[player]);

            if (electedToDie.Count != 0 && selectIonResult[player] > selectIonResult[electedToDie[0].ToString()])
            {
                electedToDie = new ArrayList();
                electedToDie.Add(player);
            }
            else if (electedToDie.Count != 0 && selectIonResult[player] == selectIonResult[electedToDie[0].ToString()])
            {
                electedToDie.Add(player);
            }
        }

        DetermineResult(electedToDie);
    }

    public void DetermineResult(ArrayList deathList)
    {
        Text text = parent.AddComponent<Text>();
        switch (deathList.Count)
        {
            case 0:
                Debug.Log("Kein Spieler wurde gewählt");
                text.text = "Kein Spieler wurde gewählt";
                break;
            case 1:
                Debug.Log("Ein Spieler wurde gewählt");
                ExecutePlayer(deathList[0].ToString());
                break;
            default:
                Debug.Log("Es wurde keine Einigung getroffen. Mehrere Spieler haben die gleiche Punktzahl");
                break;
        }
        /*
        MainMenu.ActivateImposterSelection();
        */
    }

    private void ExecutePlayer(string playerName)
    {
        GameObject player = GameObject.Find(playerName);
        player.GetComponent<PlayerStuff>().DestroyMeServerRpc();
    }

}
