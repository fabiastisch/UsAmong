using MLAPI;
using UnityEngine;
using Utils;

namespace Lobby {
    public class Lobby : MonoBehaviour {
        public GameObject lobbyManager;
        // Start is called before the first frame update
        void Start() {
            if (NetUtils.IsServer()) {
                Instantiate(lobbyManager, Vector3.zero, Quaternion.identity);
                lobbyManager.GetComponent<NetworkObject>().Spawn();
            }
        }

        // Update is called once per frame
        void Update() {
        }
    }
}