using MLAPI;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerControls : NetworkBehaviour {
        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (!IsLocalPlayer) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                
            }
            if (Input.GetKeyDown(KeyCode.Tab)) {
                
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                
            }
        }
    }
}