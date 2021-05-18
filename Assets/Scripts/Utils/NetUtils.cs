using MLAPI;
using UnityEngine;

namespace Utils {
    public class NetUtils : MonoBehaviour {
        public static bool IsConnected() {
            return NetworkManager.Singleton && NetworkManager.Singleton.IsListening;
        }
    }
}