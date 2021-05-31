using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoaderManager {
    public static void LoadLobby() {
        Debug.Log("[SceneLoaderManager] - load LOBBY");
        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
    }

    public static void LoadMainMenu() {
        Debug.Log("[SceneLoaderManager] - load MAIN menu");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public static void LoadEscMenu() {
        Debug.Log("[SceneLoaderManager] - load ESC menu");
        SceneManager.LoadScene("EscMenu", LoadSceneMode.Additive);
    }
}
