using System.Collections;
using Lobby;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour {
        public NetworkVariableBool isAliveNetVar = new NetworkVariableBool(NetUtils.Everyone, true);
        public bool isReportable = false;
        public NetworkVariableBool isImposterNetVar = new NetworkVariableBool(NetUtils.Everyone, false);
        private ArrayList deadBodys = new ArrayList();

        private void Start() {
            // Change Color if Imposter
            isImposterNetVar.OnValueChanged += (value, newValue) => {
                GameObject local = NetUtils.GetLocalObject().gameObject;
                if (local.GetComponent<PlayerLife>().isImposterNetVar.Value) {
                    if (newValue) {
                        GetComponent<PlayerStuff>().playerNameTMP.color = Color.red;
                    }
                }

                if (!newValue) {
                    GetComponent<PlayerStuff>().playerNameTMP.color = Color.white;
                }
            };
        }

        public void Kill() {
            isAliveNetVar.Value = false;
            KillServerRPC(GetComponent<NetworkObject>().OwnerClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void KillServerRPC(ulong killedPlayerId) {
            NetworkObject killedPlayer = NetworkManager.ConnectedClients[killedPlayerId].PlayerObject;
            //netObj.GetComponent<PlayerStuff>().PlayerName.Value += "[DEAD]";
            GameObject deadBody = LobbyManager.Singleton.deadPlayerObject;
            GameObject instanceDeadBody = Instantiate(deadBody, transform.position, Quaternion.identity);
            // Tp Dead player to Death Box

            instanceDeadBody.GetComponent<NetworkObject>().Spawn();
            deadBodys.Add(instanceDeadBody);

            ClientRpcParams clientRpcParams = new ClientRpcParams() {
                Send = new ClientRpcSendParams() {
                    TargetClientIds = new[] {killedPlayerId}
                }
            };
            KillClientRPC(clientRpcParams);
            updateAmountOfLivingBeings(killedPlayer);
            DetermineVictory();
        }

        [ClientRpc]
        private void KillClientRPC(ClientRpcParams clientRpcParams = default) {
            transform.position = LocalLobbyManager.Instance.deathBox.transform.position;
        }

        /**
         * On Server
         */
        public void updateAmountOfLivingBeings(NetworkObject killedPlayer) {
            LobbyManager.Singleton.DetermineNumberOfLivingCrewmatesOnServer();
            if (killedPlayer.GetComponent<PlayerLife>().isImposterNetVar.Value) // If Killed Player was imposter
            {
                LobbyManager.Singleton.MinimizeImposterNumber();
            }
        }
        
        /**
         * Determines victory by existing Crewmates or Imposter
         * 
         * On Server
         */
        public void DetermineVictory() {
            LobbyManager lobbyManager = LobbyManager.Singleton;

            if (lobbyManager.livingCrewMates.Value <= lobbyManager.impostersCount) {
                SetWinScreenClientRPC(true);
                LobbyManager.Singleton.ResetGameOnServer();
            }
            else if (lobbyManager.impostersCount == 0) {
                SetWinScreenClientRPC(false);
                LobbyManager.Singleton.ResetGameOnServer();
            }
        }

        [ClientRpc]
        private void SetWinScreenClientRPC(bool isImposterWinScreen) {
            if (isImposterWinScreen) {
                CanvasLogic.Instance.StartWinScreen(true);
            }
            else {
                CanvasLogic.Instance.StartWinScreen(false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void DestroyDeadBodyServerRPC() {
            foreach (GameObject deadBody in deadBodys) {
                Destroy(deadBody);
            }
        }
    }
}