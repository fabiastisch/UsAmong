using System.Collections.Generic;
using MLAPI;
using UnityEngine;

namespace EscMenu {
    public class EscMenu : MonoBehaviour {
        #region SingletonPattern

        private static EscMenu instance;

        public static EscMenu Instance {
            get => instance;
        }

        private void Awake() {
            if (instance == null) {
                instance = this;
            }else if (instance != this) {
                Debug.LogWarning("EscMenu already exist.");
                Destroy(gameObject);
            }
        }

        #endregion

        public GameObject escMenu;
        public GameObject optionsMenu;

        /**
        * List used to easy activate and deactivate GameObject.
        */
        private readonly List<GameObject> _list = new List<GameObject>();

        private GameObject canvas;

        // Start is called before the first frame update
        void Start() {
            if (this.transform.childCount == 1) {
                this.transform.GetChild(0).gameObject.SetActive(false);
                canvas = this.transform.GetChild(0).gameObject;
            }
            else Debug.LogError("Check this Method.. to activate Canvas in Play mode if its deactivate in editor mode");

            _list.Add(escMenu);
            _list.Add(optionsMenu);
            SetActive(null);
            //gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                canvas.SetActive(!canvas.activeSelf);
                SetActive(escMenu.activeSelf ? null : escMenu);
            }
        }

        public void Quit() {
            Application.Quit();
        }

        public void Options() {
            SetActive(optionsMenu);
        }

        public void Back() {
            SetActive(escMenu);
        }

        public void Resume() {
            SetActive(null);
        }

        public void Disonnect() {
            NetworkManager manager = NetworkManager.Singleton;
            if (!manager) {
                Application.Quit();
                return;
            }

            if (manager.IsHost) {
                manager.StopHost();
            }
            else if (manager.IsClient) {
                manager.StopClient();
            }
            else if (manager.IsServer) {
                manager.StopServer();
            }

            SceneLoaderManager.LoadMainMenu();
            //SceneManager.LoadScene("Scenes/MainMenu");
        }

        /**
         * Loop through list and set the given gameObject active, deactivate every other.
         */
        private void SetActive(GameObject gameObject) {
            foreach (var o in _list) {
                o.SetActive(o == gameObject);
            }
        }
    }
}