using Lobby;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour {
        public NetworkVariableBool isAliveNetVar = new NetworkVariableBool(NetUtils.Everyone, true);
        public bool isReportable = false;
        public NetworkVariableBool isImposterNetVar = new NetworkVariableBool(NetUtils.Everyone, false);

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
                    GetComponent<PlayerStuff>().playerNameTMP.color = Color.black;
                }
            };
        }

        public void Kill() {
            isAliveNetVar.Value = false;
            KillServerRPC(GetComponent<NetworkObject>().OwnerClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void KillServerRPC(ulong killedPlayerId) {
            NetworkObject netObj = NetworkManager.ConnectedClients[killedPlayerId].PlayerObject;
            netObj.GetComponent<PlayerStuff>().PlayerName.Value += " [DEAD]";

            GameObject deadBody = LobbyManager.Singleton.deadPlayerObject;
            GameObject instanceDeadBody = Instantiate(deadBody, transform.position, Quaternion.identity);
            // instanceDeadBody.GetComponent<PlayerLife>().isAlive = false;
            instanceDeadBody.GetComponent<NetworkObject>().Spawn();
            
            LobbyManager lobbyManager = LobbyManager.Singleton;
            lobbyManager.DetermineNumberOfLivingCrewmatesServerRPC();
            if (netObj.GetComponent<PlayerLife>().isImposterNetVar.Value)
            {
                LobbyManager.Singleton.MinimizeImposterNumber();
            }
            Debug.Log("[KillServerRPC] imostercount:" + lobbyManager.impostersCount);
            Debug.Log("[KillServerRPC] livingCrewMates: " + lobbyManager.livingCrewMates.Value);

            if (lobbyManager.impostersCount == 0 || lobbyManager.impostersCount >= lobbyManager.livingCrewMates.Value)
            {
                CanvasLogic.Instance.StartImposterWinScreen();
            }
            
            Debug.Log("[KillServerRPC]" + LobbyManager.Singleton.livingCrewMates.Value);
        }
    }
}