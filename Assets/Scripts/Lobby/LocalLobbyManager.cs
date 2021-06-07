using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby {
    public class LocalLobbyManager : MonoBehaviour {
        #region SingletonPattern

        private static LocalLobbyManager instance;

        public static LocalLobbyManager Instance {
            get => instance;
        }

        public static event Action OnSingletonReady;

        private void Awake() {
            if (instance == null) {
                instance = this;
                OnSingletonReady?.Invoke();
            }
            else if (instance != this) {
                Debug.LogWarning("LocalLobbyManager already exist.");
                Destroy(gameObject);
            }
        }

        #endregion
        
        public GameObject chatPanel;
        public GameObject textObject;
        public InputField chatBox; 
        
    }
}