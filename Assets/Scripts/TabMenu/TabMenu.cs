﻿using System;
using TMPro;
using UnityEngine;

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

            UpdateText("Not Loaded...");
            LobbyManager.OnPlayerListUpdated += list => {
                string text = "";
                foreach (string name in list) {
                    text += name + "\n";
                }

                UpdateText(text);
            };
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKey(KeyCode.Tab)) {
                if (!tab.activeSelf) {
                    tab.SetActive(true);
                }
            }
            else { // Key not pressed
                if (tab.activeSelf) {
                    tab.SetActive(false);
                }
            }
        }

        public void UpdateText(string text) {
            this.text.text = text;
        }
    }
}