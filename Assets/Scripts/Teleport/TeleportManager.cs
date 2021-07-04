using System;
using MLAPI;
using MLAPI.Logging;
using MLAPI.Messaging;
using MLAPI.Spawning;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Teleport {
    [RequireComponent(typeof(NetworkObject))]
    public class TeleportManager : NetworkBehaviour {
        #region SingletonPattern

        private static TeleportManager instance;

        public static TeleportManager Instance {
            get => instance;
        }

        public static event Action OnSingletonReady;

        private void Awake() {
            if (instance == null) {
                instance = this;
                OnSingletonReady?.Invoke();
            }
            else if (instance != this) {
                instance = this;
                Debug.LogWarning("TeleportManager already exist.");
            }
        }

        #endregion


        [ServerRpc(RequireOwnership = false)]
        public void TeleportationServerRpc(Vector3 position) {
            TeleportationClientRpc(position);
        }

        [ClientRpc]
        public void TeleportationClientRpc(Vector3 position) {
            GameObject localPlayer = getLocalPlayer();
            if (!localPlayer.GetComponent<PlayerLife>().isAliveNetVar.Value) {
                NetworkLog.LogWarningServer("[TeleportationClientRpc]: Player is not Alive: " + localPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
                return;
            }

            Vector3 random = getRandomPositionWithBounds(position);
            while (Physics2D.OverlapCircleAll(random, 3f).Length > 0)
            { 
                random = getRandomPositionWithBounds(position);
            }
            localPlayer.transform.position = random;
        }

        private Vector3 getRandomPositionWithBounds(Vector3 position) {
            return new Vector3(Random.Range(-5f + position.x, 5f + position.x),
                Random.Range(-5f + position.y, 5f + position.y), 0);
        }

        public GameObject getLocalPlayer() {
            return NetworkSpawnManager.GetLocalPlayerObject().gameObject;
        }
    }
}