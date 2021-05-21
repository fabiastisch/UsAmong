using MLAPI;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerCamera : NetworkBehaviour {
        public float initCamDistance = 30f;
        public bool isEnabled = true;

        private Camera cam;

        // Start is called before the first frame update
        void Start() {
            if (!IsLocalPlayer) {
                return;
            }

            cam = Camera.main;
            Debug.Assert(cam != null, nameof(cam) + " != null");
            cam.orthographicSize = (Screen.height / 2) / initCamDistance;
        }

        private void FixedUpdate() {
            if (!IsLocalPlayer) {
                return;
            }

            Vector3 playerPos = transform.position;
            cam.transform.position = new Vector3(playerPos.x, playerPos.y, cam.transform.position.z);
        }

        private void Update() {
            if (!IsLocalPlayer) {
                return;
            }

            if (!isEnabled) {
                return;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0) {
                float scrollFactor = scroll * 10;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scrollFactor, 1.5f, 100f);
            }
        }
    }
}