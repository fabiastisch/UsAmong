using Game;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour {
        public bool isAlive = true;
        public bool isReportable = false;


        public void Kill() {
            isAlive = false;
            //GameManager.Singleton.ChangePlayerStatusServerRpc(GetComponent<NetworkObject>().OwnerClientId, false);
            
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