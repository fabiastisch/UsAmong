using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour {
        public NetworkVariableBool isAliveNetVar = new NetworkVariableBool(NetUtils.Everyone, true);
        public bool isReportable = false;


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