using System;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using Player;
using UnityEngine;
using Utils;

namespace Game {
    // Script requires the GameObject to have a NetworkObject component
    [RequireComponent(typeof(NetworkObject))]
    public class GameManager : NetworkBehaviour {
        #region SingletonPattern

        public static GameManager Singleton { get; private set; }

        public static event Action OnSingletonReady;

        private void Awake() {
            if (Singleton == null) {
                Singleton = this;
                OnSingletonReady?.Invoke();
            }
            else if (Singleton != this) {
                Debug.LogWarning("GameManager already exist.");
                Destroy(this);
            }
        }

        #endregion

        public List<ulong> connectionList = new List<ulong>();
        public List<ulong> deadPlayerList = new List<ulong>();
        public List<ulong> alivePlayerList = new List<ulong>();

        private void Start() {
            if (!NetUtils.IsConnected()) {
                return;
            }

            if (NetUtils.IsServer()) {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                connectionList = new List<ulong>(NetworkManager.ConnectedClients.Keys);
            }
        }

        private void OnClientDisconnect(ulong obj) {
            connectionList = new List<ulong>(NetworkManager.ConnectedClients.Keys);
        }

        private void OnClientConnected(ulong obj) {
            connectionList = new List<ulong>(NetworkManager.ConnectedClients.Keys);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetAllPlayerAliveServerRpc() {
            deadPlayerList = new List<ulong>();
            alivePlayerList = connectionList;


            foreach (ulong id in connectionList) {
                NetworkObject netObj = NetworkManager.ConnectedClients[id].PlayerObject;
                if (netObj.GetComponent<PlayerStuff>().PlayerName.Value.EndsWith("[DEAD]")) {
                    netObj.GetComponent<PlayerStuff>().PlayerName.Value = netObj.GetComponent<PlayerStuff>().PlayerName.Value.Replace("[DEAD]", "");
                }
            }


            /*foreach (ulong id in connectionList) {
                NetworkObject netObj = NetworkManager.ConnectedClients[id].PlayerObject;
                foreach (ulong otherId in connectionList) {
                    if (!netObj.IsNetworkVisibleTo(otherId)) {
                        netObj.NetworkShow(otherId);
                        
                    }
                }                
            }*/
        }


        [ServerRpc(RequireOwnership = false)]
        public void ChangePlayerStatusServerRpc(ulong clientId, bool alive) {
            if (alive) {
                if (deadPlayerList.Contains(clientId)) {
                    deadPlayerList.Remove(clientId);
                }

                if (!alivePlayerList.Contains(clientId)) {
                    alivePlayerList.Add(clientId);
                }
            }
            else {
                if (alivePlayerList.Contains(clientId)) {
                    alivePlayerList.Remove(clientId);
                }

                if (!deadPlayerList.Contains(clientId)) {
                    deadPlayerList.Add(clientId);
                }
            }

            NetworkObject netObj = NetworkManager.ConnectedClients[clientId].PlayerObject;
            if (alive) {
                if (netObj.GetComponent<PlayerStuff>().PlayerName.Value.EndsWith("[DEAD]")) {
                    netObj.GetComponent<PlayerStuff>().PlayerName.Value.Replace("[DEAD]", "");
                }
            }
            else {
                netObj.GetComponent<PlayerStuff>().PlayerName.Value += " [DEAD]";
            }

            /*NetworkObject netObj = NetworkManager.ConnectedClients[clientId].PlayerObject;
            foreach (ulong id in alivePlayerList) {
                NetworkObject otherNetObj = NetworkManager.ConnectedClients[id].PlayerObject;

                if (otherNetObj.IsNetworkVisibleTo(clientId)) { 
                    otherNetObj.NetworkHide(clientId);
                }
                
            }
            foreach (ulong id in deadPlayerList) {
                if (!netObj.IsNetworkVisibleTo(id)) { 
                    netObj.NetworkShow(id);
                }
            }*/
        }
    }
}