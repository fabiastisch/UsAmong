using MLAPI;
using UnityEngine;

public class AutoLoadMainMenu : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsListening) {
            SceneLoaderManager.LoadMainMenu();
        }
    }
    
}