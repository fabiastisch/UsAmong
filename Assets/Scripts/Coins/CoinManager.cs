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
    
    public NetworkVariableInt remainingCoinsNetVar = new NetworkVariableInt(NetUtils.Everyone, 0);
    

    public GameObject coinObject;
    private TMP_Text text;

    private int coinsPerPlayer = 20;
    private bool isImposter;

    // Start is called before the first frame update

    public void Start()
    {
        text = CanvasLogic.Instance.coinCounterObj.GetComponent<TMP_Text>();
    }
    
    public void Update()
    {
        text.text = "Coins to Collect: " + remainingCoinsNetVar.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetermineNumberOfCoinsServerRPC()
    {
        Debug.Log(LobbyManager.Singleton.livingCrewMates.Value );
        remainingCoinsNetVar.Value = LobbyManager.Singleton.livingCrewMates.Value * coinsPerPlayer;
        SpawnNewCoinsClientRPC(coinsPerPlayer);
    }
    
    public void determineRemainingCoins(GameObject o, GameObject other)
    {
        if (other.GetComponent<PlayerStuff>().IsLocalPlayer)
        {
            isImposter = other.GetComponent<PlayerLife>().isImposterNetVar.Value;

            if (isImposter)
            {
                int numberOfNewCoins = 10;
                Debug.Log("[determineRemainingCoins]: " + LobbyManager.Singleton.livingCrewMates.Value);
                remainingCoinsNetVar.Value = remainingCoinsNetVar.Value +
                                             numberOfNewCoins * LobbyManager.Singleton.livingCrewMates.Value;
                SpawnNewCoinsServerRpc(numberOfNewCoins);
            }
            else
            {
                remainingCoinsNetVar.Value -= 1;
            }

            if (remainingCoinsNetVar.Value == 0)
            {
                CanvasLogic.Instance.StartPlayerWinScreen();
            }

            Destroy(o);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnNewCoinsServerRpc(int numberOfNewCoins)
    {
        SpawnNewCoinsClientRPC(numberOfNewCoins);
    }
    
    [ClientRpc]
    public void SpawnNewCoinsClientRPC(int amountOfCoins)
    {
        if (!isImposter)
        {
            for (int i = 0; i < amountOfCoins; i++)
            {
                Vector3 random = new Vector3(Random.Range(-160f, 40f), Random.Range(-20f, -140f), 0);
                Instantiate(coinObject, random, Quaternion.identity);
            } 
        }
    }
}
