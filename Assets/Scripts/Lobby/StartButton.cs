using UnityEngine;

namespace Lobby {
    public class StartButton : MonoBehaviour {
        public Transform start;

        public GameObject button;

        public void OnStartButtonClicked() {
            Debug.Log("Start Button CLICKED");
            //SceneLoaderManager.LoadGame();
            LobbyManager.Singleton.StartGameServerRpc(start.position);
            //LobbyManager.Singleton.getLocalPlayer().transform.position = start.position;
            button.SetActive(false);
        }
    }
}