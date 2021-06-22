using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using Player;
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
    private int coinsPerPlayer = 100;
    public NetworkVariableInt remainingCoinsNetVar = new NetworkVariableInt(NetUtils.Everyone, 0);
    
    public NetworkList<string> playerList = new NetworkList<string>(NetUtils.Everyone);


    // Start is called before the first frame update


    [ServerRpc(RequireOwnership = false)]
    public void DetermineNumberOfCoinsServerRPC()
    {
        remainingCoinsNetVar.Value = GetPlayers().Count() * coinsPerPlayer;
        Debug.Log(remainingCoinsNetVar.Value);
        SpawnCoinsClientRPC();
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
    public void minimizeRemainingCoins(GameObject o)
    {
        remainingCoinsNetVar.Value -= 1;
        Debug.Log("[" + nameof(minimizeRemainingCoins) + "]" + remainingCoinsNetVar.Value);
        Destroy(o);
    }
    
    [ClientRpc]
    public void SpawnCoinsClientRPC() {
        for (int i = 0; i < coinsPerPlayer; i++)
        {
            Vector3 random = new Vector3(Random.Range(-160f, 40f), Random.Range(-20f, -140f), 0);
            GameObject coin = Instantiate(coinObject, random, Quaternion.identity);
        }
    }
    
    
    public IEnumerable<string> GetPlayers() {
        return this.playerList;
    }

}
