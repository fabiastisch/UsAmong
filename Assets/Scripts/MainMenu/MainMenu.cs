using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Transports.UNET;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour {
    public GameObject startMenu;
    public GameObject playMenu;
    public GameObject optionMenu;
    
    private String username;
    
    public TMP_InputField ipAddressInput;
    public TMP_InputField usernameInput;

    /**
        * List used to easy activate and deactivate GameObject.
        */
    private readonly List<GameObject> _list = new List<GameObject>();

    public void ActivateMainMenu() {
        SetActive(startMenu);
    }

    public void ActivateStartMenu() {
        SetActive(playMenu);
    }

    public void ActivateOptionsMenu() {
        SetActive(optionMenu);
    }

    /**
         * Method Called once before the first frame will be rendered.
         * Add every Menu to _list.
         * Activate the Main Menu.
         */
    private void Start() {
        _list.Add(startMenu);
        _list.Add(playMenu);
        _list.Add(optionMenu);

        SetActive(startMenu);
    }

    /**
         * Loop through list and set the given gameObject active, deactivate every other.
         */
    private void SetActive(GameObject gameObject) {
        foreach (var o in _list) {
            o.SetActive(o == gameObject);
        }
    }
    
    public void QuitGame() {
        Application.Quit();
    }
    
    public void PlayGame() {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        SceneLoaderManager.LoadLobby();
    }
    
    public void Host() {
        UpdateSettings();
        NetworkManager.Singleton.StartHost();
        //connect.SetActive(false);
        //disconnect.SetActive(true);
        PlayGame();
    }
    
    public void Join() {
        // NetworkSceneManager.SwitchScene(SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1).name);
        UpdateSettings();
        NetworkManager.Singleton.StartClient();
        //connect.SetActive(false);
        //disconnect.SetActive(true); 
        PlayGame();
    }
    
    public void Server() {
        UpdateSettings();
        NetworkManager.Singleton.StartServer();
    }

    public void Disconnect() {
        NetworkManager manager = NetworkManager.Singleton;

        if (manager.IsHost) {
            manager.StopHost();
        }
        else if (manager.IsClient) {
            manager.StopClient();
        }
        else if (manager.IsServer) {
            manager.StopServer();
        }

        SceneManager.LoadScene("Scenes/MainMenu");
    }
    
    private void UpdateSettings() {
        username = usernameInput.text;
        // UpdateAdress();
    }
    private void UpdateAdress() {
        UNetTransport transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        Debug.Log(ipAddressInput);
        string ip = ipAddressInput.text;
        int lastIndexOfColon = ip.LastIndexOf(':');
        string host = ip.Substring(0, lastIndexOfColon);
        string port = ip.Substring(lastIndexOfColon + 1);
        transport.ServerListenPort = Convert.ToInt32(port);
        transport.ConnectAddress = host;
        transport.ConnectPort = Convert.ToInt32(port);
    }

    // Update is called once per frame
    void Update() {
    }
}