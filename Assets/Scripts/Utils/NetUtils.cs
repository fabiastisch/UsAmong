using MLAPI;
using UnityEngine;

namespace Utils {
    public class NetUtils : MonoBehaviour {
        public static bool IsConnected() {
            return NetworkManager.Singleton && NetworkManager.Singleton.IsListening;
        }

        public static bool IsServer() {
            return NetworkManager.Singleton && NetworkManager.Singleton.IsServer;
        }

        public static bool IsHost() {
            return NetworkManager.Singleton && NetworkManager.Singleton.IsHost;
        }

        public static bool IsClient() {
            return NetworkManager.Singleton && NetworkManager.Singleton.IsClient;
        }
    }
}