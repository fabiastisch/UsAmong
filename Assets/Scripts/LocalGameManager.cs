using System;
using MLAPI;
using MLAPI.Spawning;
using Player;
using UnityEngine;

public class LocalGameManager : MonoBehaviour {
    public string playerName;

    #region SingletonPattern

    public static LocalGameManager Singleton { get; private set; }

    internal static event Action OnSingletonReady;

    private void Awake() {
        if (Singleton == null) {
            Singleton = this;
            OnSingletonReady?.Invoke();
        }
        else if (Singleton != this) {
            Debug.LogWarning("Lobby Manager already exist.");
            Destroy(this);
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start() {
        LobbyManager.OnLocalPlayerSpawned += OnLocalPlayerSpawned;
    }

    private void OnLocalPlayerSpawned(ulong clientId) {
        if (!NetworkManager.Singleton.LocalClientId.Equals(clientId)) {
            return;
        }

        NetworkObject localPlayerObject = NetworkSpawnManager.GetLocalPlayerObject();
        Debug.Log("OnLocalPlayerSpawned, " + localPlayerObject);
        if (localPlayerObject) {
            PlayerStuff playerStuff = localPlayerObject.gameObject.GetComponent<PlayerStuff>();
            
            if (playerStuff) {
                playerStuff.PlayerName.Value = playerName;
            }
            else {
                Debug.LogError("PlayerStuff is null");
            }
        }
        else {
            Debug.LogError("LocalPlayerObject is null");
        }

        // TODO: error?
    }
}