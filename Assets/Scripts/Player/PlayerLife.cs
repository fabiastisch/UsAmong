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
        public NetworkVariableBool isImposter = new NetworkVariableBool(NetUtils.Everyone, false);

        private void Start() {
            // Change Color if Imposter
            isImposter.OnValueChanged += (value, newValue) => {
                GameObject local = NetUtils.GetLocalObject().gameObject;
                if (local.GetComponent<PlayerLife>().isImposter.Value) {
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
        }
    }
}