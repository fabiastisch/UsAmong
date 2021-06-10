using System;
using MLAPI;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerControls : NetworkBehaviour {
        public float killRadius = 30;

        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            if (!IsLocalPlayer) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                // Kill Imposter Only
                Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), killRadius, LayerMask.GetMask("Player"));
                Debug.Log("Collider size: " + colliders.Length);
                Array.Sort(colliders,
                    (collider1, collider2) => (int) (UtilsUnity.getDistanceBetweenGameObjects(collider1.gameObject, gameObject) -
                                                     UtilsUnity.getDistanceBetweenGameObjects(collider2.gameObject, gameObject)));
                if (colliders.Length > 1) {
                    Debug.Log("'KILL'" + colliders[1].gameObject.transform.parent.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                // Report
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                // Use
            }
        }
    }
}