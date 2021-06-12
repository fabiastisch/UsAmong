using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour {
        
        public bool isAlive = true;

        public void Kill() {
            isAlive = false;
            KillServerRPC();
        }

        [ServerRpc(RequireOwnership = false)]
        private void KillServerRPC() {
            GameObject deadBody = LobbyManager.Singleton.deadPlayerObject;
            GameObject instanceDeadBody = Instantiate(deadBody, transform.position, Quaternion.identity);
            instanceDeadBody.GetComponent<NetworkObject>().Spawn();
        }
        
    }
}