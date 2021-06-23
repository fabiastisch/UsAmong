using System;
using System.Collections.Generic;
using System.Linq;
using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
using TMPro;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class CoinManager : NetworkBehaviour
{

    #region SingletonPattern

    private static CoinManager instance;


    public static CoinManager Instance {
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

    public GameObject coinObject;
    private int coinsPerPlayer = 2;
    public NetworkVariableInt remainingCoinsNetVar = new NetworkVariableInt(NetUtils.Everyone, 0);
    public Canvas canvas;
    public NetworkList<string> playerList = new NetworkList<string>(NetUtils.Everyone);
    private Canvas canvasobject;
    private TMP_Text text;

        // Start is called before the first frame update

    public void Start()
    {
        canvasobject = Instantiate(canvas, new Vector3(0,0,0), Quaternion.identity);
        text = canvasobject.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    
    public void Update()
    {
        text.text = "Coins to Collect: " + remainingCoinsNetVar.Value;;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetermineNumberOfCoinsServerRPC()
    {
        remainingCoinsNetVar.Value = GetPlayers().Count() * coinsPerPlayer;
        SpawnElementsClientRPC();
    }
    
    public void minimizeRemainingCoins(GameObject o)
    {
        remainingCoinsNetVar.Value -= 1;
        if (remainingCoinsNetVar.Value == 0)
        {
            CanvasLogic.Instance.StartPlayerWinScreen();
        }
        Destroy(o);
    }
    
    [ClientRpc]
    public void SpawnElementsClientRPC() 
    {
        for (int i = 0; i < coinsPerPlayer; i++)
        {
            Vector3 random = new Vector3(Random.Range(-160f, 40f), Random.Range(-20f, -140f), 0);
            GameObject coin = Instantiate(coinObject, random, Quaternion.identity);
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerServerRPC() {
        Debug.Log("[CoinManager] SetPlayerServerRPC");
        playerList.Clear();
        var nameList =
            NetworkManager.Singleton.ConnectedClientsList.ConvertAll<string>(client =>
                client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value);
        foreach (string name in nameList) {
            playerList.Add(name);
        }
    }
    
    public IEnumerable<string> GetPlayers() {
        return this.playerList;
    }

}
