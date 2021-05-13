using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadMainMenu : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsListening) {
            SceneLoaderManager.LoadMainMenu();
        }
    }
    
}