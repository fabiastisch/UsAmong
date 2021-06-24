using System;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerStuff : NetworkBehaviour {
        
        public TMP_Text playerNameTMP;

        [SerializeField]
        private NetworkVariableString playerName = new NetworkVariableString(NetUtils.Everyone, "undefined");

        public NetworkVariableString PlayerName => playerName;
        
        private void OnEnable() {
            PlayerName.OnValueChanged += OnPlayerNameChanged;
        }

        private void OnDisable() {
            PlayerName.OnValueChanged -= OnPlayerNameChanged;
        }

        private void OnPlayerNameChanged(string previousvalue, string newvalue) {
            if (playerNameTMP) {
                playerNameTMP.text = newvalue;
            }
            else {
                Debug.LogWarning("[PlayerStuff] playerNameTMP : " + playerNameTMP);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void DestroyMeServerRpc() {
            Debug.Log("[DestroyMeServerRPC]");
            //NetworkManager.Destroy(GetComponent<NetworkObject>());
            GetComponent<NetworkObject>().Despawn(true);
            LobbyManager.Singleton.DetermineNumberOfLivingCrewmatesServerRPC();
        }
        
        public bool isLocalPlayer()
        {
            return this.IsLocalPlayer;
        }
    }
}