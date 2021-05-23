using System;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Connection;
using Player;
using TMPro;
using UnityEngine;
using Utils;

namespace TabMenu {
    public class TabMenu : MonoBehaviour {
        public TMP_Text text;
        private GameObject tab;

        #region SingletonPattern

        private static TabMenu instance;

        public static TabMenu Instance {
            get => instance;
        }

        public static event Action OnSingletonReady;

        private void Awake() {
            if (instance == null) {
                instance = this;
                OnSingletonReady?.Invoke();
            }
            else if (instance != this) {
                Debug.LogWarning("TabMenu already exist.");
                Destroy(gameObject);
            }
        }

        #endregion


        void Start() {
            if (this.transform.childCount == 1) {
                tab = this.transform.GetChild(0).gameObject;
                tab.gameObject.SetActive(false);
            }
            else Debug.LogError("Check this Method.. to activate Canvas in Play mode if its deactivate in editor mode");

           
        }

        private void LobbyManagerOnOnPlayerSpawned(ulong obj) {
            Debug.Log(obj + " spawned...");
            MyNetworkManagerOnOnClientListChange();

        }

        private void MyNetworkManagerOnOnClientListChange() {
            Debug.Log("ClientList Changed to: "+ MyNetworkManager.Instance.clientlist.Count.ToString());
            List<NetworkClient> list = MyNetworkManager.Instance.clientlist;
            string names = "";
            foreach (NetworkClient client in list) {
                names += client.PlayerObject.GetComponent<PlayerStuff>().PlayerName.Value;
                names += "\n";
            }
            UpdateText(names);
        }

        // Update is called once per frame
        void Update() {
            /*
             if (Input.GetKeyDown(KeyCode.Tab)) {
             
                tab.SetActive(!tab.activeSelf);
                MyNetworkManagerOnOnClientListChange();
            }
            */
        }

        public void UpdateText(string text) {
            this.text.text = text;
        }
    }
}