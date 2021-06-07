using MLAPI;
using UnityEngine;
using Utils;

namespace Lobby {
    public class Lobby : MonoBehaviour {
        public GameObject lobbyManager;
        public GameObject gameManager;

        public GameObject canvas;

        // Start is called before the first frame update
        void Start() {
            if (NetUtils.IsServer()) {
                GameObject lobbyManagerInstance = Instantiate(lobbyManager, Vector3.zero, Quaternion.identity);
                lobbyManagerInstance.GetComponent<NetworkObject>().Spawn();
            }

            canvas.SetActive(true);
        }

        // Update is called once per frame
        void Update() {
        }
    }
}