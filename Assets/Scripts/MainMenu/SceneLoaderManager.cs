using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoaderManager {
    public static void LoadLobby() {
        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive);
    }

    public static void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
