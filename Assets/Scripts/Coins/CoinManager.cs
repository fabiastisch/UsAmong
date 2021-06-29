using System;
using System.Collections;
using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using Player;
using TMPro;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class CoinManager : NetworkBehaviour {
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
            Debug.LogWarning("CoinManager already exist.");
            //Destroy(gameObject);
        }
    }

    #endregion

    public NetworkVariableInt remainingCoinsNetVar = new NetworkVariableInt(NetUtils.Everyone, 0);
    public GameObject coinObject;

    private TMP_Text text;
    private int coinsPerPlayer = 20;
    private bool isImposter;
    private ArrayList allCoins = new ArrayList();
    private NetworkVariableInt totalCoins = new NetworkVariableInt(NetUtils.Everyone, 0);


    // Start is called before the first frame update


    [ServerRpc(RequireOwnership = false)]
    public void DetermineNumberOfCoinsServerRPC() {
        totalCoins.Value = LobbyManager.Singleton.livingCrewMates.Value * coinsPerPlayer;
        remainingCoinsNetVar.Value = totalCoins.Value;
        SpawnNewCoinsClientRPC(coinsPerPlayer);
    }

    public void DetermineRemainingCoins(GameObject coin, GameObject player) {
        if (player.GetComponent<PlayerStuff>().IsLocalPlayer) {
            isImposter = player.GetComponent<PlayerLife>().isImposterNetVar.Value;

            if (isImposter) {
                int numberOfNewCoins = 10 * LobbyManager.Singleton.livingCrewMates.Value;
                remainingCoinsNetVar.Value += numberOfNewCoins;
                totalCoins.Value += numberOfNewCoins;
                SpawnNewCoinsServerRpc(numberOfNewCoins);
            }
            else {
                remainingCoinsNetVar.Value--;
            }

            if (remainingCoinsNetVar.Value == 0) {
                CanvasLogic.Instance.StartCrewMatesWinScreen();
                LobbyManager.Singleton.ResetGameServerRpc();
            }

            CanvasLogic.Instance.SetCoinBarValue(-(remainingCoinsNetVar.Value - totalCoins.Value));
            Destroy(coin);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnNewCoinsServerRpc(int numberOfNewCoins) {
        SpawnNewCoinsClientRPC(numberOfNewCoins);
    }

    /**
     * Spawns an 'amoutOfCoins' number of Coins on each client, that is not an Imposter
     */
    [ClientRpc]
    public void SpawnNewCoinsClientRPC(int amountOfCoins) {
        CanvasLogic.Instance.SetCoinbarMaxValue(totalCoins.Value);
        if (!isImposter) {
            for (int i = 0; i < amountOfCoins; i++) {
                Vector2 random = new Vector2(Random.Range(-160f, 40f), Random.Range(-20f, -140f));
                while (Physics2D.OverlapCircleAll(random, 1f).Length > 0) {
                    random = new Vector3(Random.Range(-160f, 40f), Random.Range(-20f, -140f));
                }

                GameObject coin = Instantiate(coinObject, random, Quaternion.identity);
                allCoins.Add(coin);
            }
        }
    }

    public void DestroyAllLocalCoins() {
        foreach (GameObject coin in allCoins) {
            Destroy(coin);
        }
    }
}