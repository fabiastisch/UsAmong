using System;
using System.Collections.Generic;
using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

// Script requires the GameObject to have a NetworkObject component
namespace Chat {
    [RequireComponent(typeof(NetworkObject))]
    public class ChatManager : NetworkBehaviour {
        public int maxMessages = 25;

        public GameObject chatPanel;
        public GameObject textObject;
        public InputField chatBox;

        private readonly List<GameObject> chatList = new List<GameObject>();

        private void Start() {
            if (NetUtils.IsServer()) {
                GetComponent<NetworkObject>().Spawn();
            }
            else {
                chatPanel = LocalLobbyManager.Instance.chatPanel;
                textObject = LocalLobbyManager.Instance.textObject;
                chatBox = LocalLobbyManager.Instance.chatBox;
            }
        }

        private void MessageListOnOnListChanged(NetworkListEvent<string> changeevent) {
            Debug.Log("MessageListOnOnListChanged");
        }

        // Update is called once per frame
        void Update() {
            if (chatBox.text != "") {
                if (Input.GetKeyDown(KeyCode.Return)) {
                   // sendMessageToChat(chatBox.text);
                   SendMessageSendServerRPC(LocalGameManager.Singleton.playerName, chatBox.text);
                    chatBox.text = "";
                }
            }
            else {
                if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return)) {
                    chatBox.ActivateInputField();
                }
            }

            if (!chatBox.isFocused) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                   // sendMessageToChat("You pressed the space key");
                    Debug.Log("Space");
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendMessageSendServerRPC(string playerName, string chatBoxText) {
            OnMessageSendClientRPC(playerName, chatBoxText);
        }

        [ClientRpc]
        public void OnMessageSendClientRPC(string playerName, string chatBoxText) {
            if (chatList.Count >= maxMessages) {
                Debug.Log("Destory last object");
                Destroy(chatList[0].gameObject);
                chatList.Remove(chatList[0]);
            }
            GameObject newText = Instantiate(textObject, chatPanel.transform);
            Text text = newText.GetComponent<Text>();
            text.text = playerName+ ": " + chatBoxText;
            
            chatList.Add(newText);
        }
    }
}