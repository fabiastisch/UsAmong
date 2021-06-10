using UnityEngine;

public class AutoLoadMainMenu : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        if (!Utils.NetUtils.IsConnected()) {
            SceneLoaderManager.LoadMainMenu();
        }
    }
    
}