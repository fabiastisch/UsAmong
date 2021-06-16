using Game;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour {
        public bool isAlive = true;
        public bool isReportable = false;


        public void Kill() {
            isAlive = false;
            GameManager.Singleton.ChangePlayerStatusServerRpc(NetUtils.LocalClientId, false);
            KillServerRPC();
        }

        [ServerRpc(RequireOwnership = false)]
        private void KillServerRPC() {
            GameObject deadBody = LobbyManager.Singleton.deadPlayerObject;
            GameObject instanceDeadBody = Instantiate(deadBody, transform.position, Quaternion.identity);
            // instanceDeadBody.GetComponent<PlayerLife>().isAlive = false;
            instanceDeadBody.GetComponent<NetworkObject>().Spawn();
            
            
        }
    }
}