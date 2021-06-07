using System;
using System.Collections;
using System.Collections.Generic;
using Chat;
using Lobby;
using MLAPI;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

// Script requires the GameObject to have a NetworkObject component
[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour {
    public int maxMessages = 25;

    private NetworkList<String> messageList = new NetworkList<String>(new MLAPI.NetworkVariable.NetworkVariableSettings() {
        ReadPermission = MLAPI.NetworkVariable.NetworkVariablePermission.Everyone,
        WritePermission = MLAPI.NetworkVariable.NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<String>());


    public GameObject chatPanel;
    public GameObject textObject;
    public InputField chatBox;

    private void Start() {
        if (NetUtils.IsServer()) {
            GetComponent<NetworkObject>().Spawn();
        }
        else {
            chatPanel = LocalLobbyManager.Instance.chatPanel;
            textObject = LocalLobbyManager.Instance.textObject;
            chatBox = LocalLobbyManager.Instance.chatBox;
        }
        messageList.OnListChanged += MessageListOnOnListChanged;
    }

    private void MessageListOnOnListChanged(NetworkListEvent<string> changeevent) {
        Debug.Log("MessageListOnOnListChanged");
    }

    // Update is called once per frame
    void Update() {
        if (chatBox.text != "") {
            if (Input.GetKeyDown(KeyCode.Return)) {
                sendMessageToChat(chatBox.text);
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
                sendMessageToChat("You pressed the space key");
                Debug.Log("Space");
            }
        }
    }

    public void sendMessageToChat(String text)
    {

        if (messageList.Count >= maxMessages)
        {
           // Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newText.GetComponent<Text>().text = text;
       // newMessage.textObject = newText.GetComponent<Text>();

       // newMessage.textObject.text = newMessage.text;
        
        messageList.Add(text);
    }
}