using System;
using UnityEngine;

namespace Lobby {
    public class StartButton : MonoBehaviour {
        #region SingletonPattern

        private static StartButton instance;

        public static StartButton Instance {
            get => instance;
        }

        public static event Action OnSingletonReady;

        private void Awake() {
            if (instance == null) {
                instance = this;
                OnSingletonReady?.Invoke();
            }
            else if (instance != this) {
                Debug.LogWarning("StartButton already exist.");
                Destroy(gameObject);
            }
        }

        #endregion
        
        public Transform start;

        public GameObject button;

        public void OnStartButtonClicked() {
            Debug.Log("Start Button CLICKED");
            //SceneLoaderManager.LoadGame();
            LobbyManager.Singleton.StartGameServerRpc(start.position);
            //LobbyManager.Singleton.getLocalPlayer().transform.position = start.position;
        }

        public void SetStartButtonActive(bool value) {
            button.SetActive(value);
        }
    }
}