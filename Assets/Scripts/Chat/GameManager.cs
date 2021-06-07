using System;
using System.Collections;
using System.Collections.Generic;
using Chat;
using MLAPI;
using MLAPI.NetworkVariable.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{

    public int maxMessages = 25;
    
    private NetworkList<Message> messageList = new NetworkList<Message>(new MLAPI.NetworkVariable.NetworkVariableSettings()
    {
        ReadPermission = MLAPI.NetworkVariable.NetworkVariablePermission.Everyone,
        WritePermission = MLAPI.NetworkVariable.NetworkVariablePermission.Everyone,
        SendTickrate = 5
    }, new List<Message>());
    
    
    public GameObject chatPanel;
    public GameObject textObject;
    public InputField chatBox;
    

    // Update is called once per frame
    void Update()
    {
        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                sendMessageToChat(chatBox.text);
                chatBox.text = "";
            }
        }else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }

        if (!chatBox.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                sendMessageToChat("You pressed the space key");
                Debug.Log("Space");
            }
        }
    }

    public void sendMessageToChat(String text)
    {

        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();

        newMessage.textObject.text = newMessage.text;
        
        messageList.Add(newMessage);
    }
}
