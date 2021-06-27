using System;
using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using Player;
using UnityEngine;
using Utils;

namespace Teleport
{
    [RequireComponent(typeof(NetworkObject))]
    public class TeleportManager : NetworkBehaviour
    {
        
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
        public void TeleportationClientRpc(Vector3 position)
        {
            GameObject localPlayer = getLocalPlayer();
            localPlayer.transform.position = position;
        }
        
        public GameObject getLocalPlayer() {
            return NetworkSpawnManager.GetLocalPlayerObject().gameObject;
        }
    }
}