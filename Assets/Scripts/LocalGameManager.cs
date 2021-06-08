﻿using System;
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
        NetworkObject o = NetworkSpawnManager.GetLocalPlayerObject();
        Debug.Log("OnLocalPlayerSpawned, " + o);
        o.gameObject.GetComponent<PlayerStuff>().PlayerName.Value = playerName;
    }

}