using System;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
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
                Collider2D[] colliders = CheckSorroundingArea();
                
                foreach (Collider2D playerCollider in colliders) {
                    
                    GameObject otherPlayer = playerCollider.transform.parent.gameObject;
                    if (!CheckForPlayer(otherPlayer))
                    {
                        continue;
                    }
                    
                    if (!otherPlayer.Equals(gameObject)) {
                        PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                        if (otherPlayerLife) {
                            if (!otherPlayerLife.isAlive) {
                                Debug.Log("[ON KILL] Other Player isn't Alive");
                                break;
                            }
                            
                            otherPlayerLife.Kill();
                        }
                        else {
                            Debug.LogWarning("[ON KILL] OtherPlayerLife is null? " + otherPlayerLife);
                        }
                        
                        break;
                    }
                }
                if (colliders.Length > 1) {
                    Debug.Log("[ON KILL]: " + colliders[1].gameObject.transform.parent.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                Collider2D[] colliders = CheckSorroundingArea();

                foreach (Collider2D playerCollider in colliders)
                {
                    GameObject otherPlayer = playerCollider.transform.parent.gameObject;
                    
                    if (!CheckForPlayer(otherPlayer))
                    {
                        continue;
                    }
                    
                    if (!otherPlayer.Equals(gameObject)) {
                        PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                        if (otherPlayerLife) {
                            if (!otherPlayerLife.isAlive && !otherPlayerLife.isReported)
                            {
                                otherPlayerLife.isReported = true;
                                Debug.Log("[ON REPORT] report player: " + otherPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
                                StartConsultationServerRpc(Vector3.zero);
                                break;
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Use
            }
        }

        private bool CheckForPlayer(GameObject otherGameObject)
        {
            if (!otherGameObject.CompareTag("Player")) {
                Debug.Log("[CheckForPlayer] Object is not Player?: " + otherGameObject);
                return false;
            }
            return true;
        }

        private Collider2D[] CheckSorroundingArea()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), killRadius, LayerMask.GetMask("Player"));
            Debug.Log("[CheckSorroundingArea] Collider size: " + colliders.Length);
            Array.Sort(colliders,
                (collider1, collider2) => (int) (UtilsUnity.getDistanceBetweenGameObjects(collider1.gameObject, gameObject) -
                                                 UtilsUnity.getDistanceBetweenGameObjects(collider2.gameObject, gameObject)));
            return colliders;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void StartConsultationServerRpc(Vector3 consultationPosition) {
            StartConsultationClientRpc(consultationPosition);
        }

        [ClientRpc]
        public void StartConsultationClientRpc(Vector3 consultationPosition) {
            GameObject localPlayer = getLocalPlayer();
            PlayerLife playerLife = localPlayer.GetComponent<PlayerLife>();
            if (playerLife.isAlive)
            {
                Debug.Log("move player: " + localPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
                localPlayer.transform.position = consultationPosition;
            }
        }
        
        public GameObject getLocalPlayer() {
            return NetworkSpawnManager.GetLocalPlayerObject().gameObject;;
        }
    }
}