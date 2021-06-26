using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lobby;
using MLAPI;
using MLAPI.Spawning;
using MLAPI.Transports.Tasks;
using MLAPI.Transports.UNET;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class MainMenu : MonoBehaviour {
    public GameObject startMenu;
    public GameObject playMenu;
    public GameObject optionMenu;
    
    /*
    public GameObject imposterSelection;
    */
    
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
        /*
        _list.Add(imposterSelection);
        */

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
        SceneLoaderManager.LoadEscMenu();
    }

    public void Host() {
        UpdateSettings();
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.StartHost();
        //connect.SetActive(false);
        //disconnect.SetActive(true);
        PlayGame();
    }

    public void Join() {
        // NetworkSceneManager.SwitchScene(SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1).name);
        UpdateSettings();
        SocketTasks socketTasks = NetworkManager.Singleton.StartClient();
        CheckJoin(socketTasks);
        //connect.SetActive(false);
        //disconnect.SetActive(true); 
    }

    private async Task CheckJoin(SocketTasks socketTasks ) {
        while (!socketTasks.IsDone) {
            Debug.Log("connecting...");
            await Task.Yield();
        }
        if ( NetUtils.IsConnected()) {
            PlayGame();
        }
        else {
            Debug.Log("Connection Error");
        }
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
        LocalGameManager.Singleton.playerName = usernameInput.text;
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

    /**
     * Happen on server
     */
    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback) {
        Debug.Log("Approving a connection");
        // logic
        CanvasLogic canvasLogic = CanvasLogic.Instance;
        bool approve = !(canvasLogic && canvasLogic.inGame);
        bool createPlayerObject = false;
        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
        ulong? prefabHash = NetworkSpawnManager.GetPrefabHashFromGenerator("Player");

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        Vector3? positionToSpawnWith = Vector3.zero;
        Quaternion? rotationToSpawnWith = Quaternion.identity;

        callback(createPlayerObject, prefabHash, approve, positionToSpawnWith, rotationToSpawnWith);
    }
}