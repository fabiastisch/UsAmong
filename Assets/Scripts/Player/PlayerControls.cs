using System;
using Lobby;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using Teleport;
using UnityEngine;
using UnityEngine.Analytics;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerControls : NetworkBehaviour {
        public float killRadius = 3;
        private PlayerLife _playerLife;

        // Start is called before the first frame update
        void Start() {
            _playerLife = GetComponent<PlayerLife>();
        }

        // Update is called once per frame
        void Update() {
            if (!IsLocalPlayer) {
                return;
            }

            if (!_playerLife.isAliveNetVar.Value) {
                return;
            }

            CheckForReport();
            
            // if imposter
            if (GetComponent<PlayerLife>().isImposterNetVar.Value) {
                CheckForKill();

                if (Input.GetKeyDown(KeyCode.Q)) {
                    // Kill Imposter Only
                    PerformKill();
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                // Report
                PerformReport();
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                // Use
            }
        }

        public void PerformReport() {
            Collider2D[] colliders = CheckSorroundingArea();

            foreach (Collider2D playerCollider in colliders) {
                GameObject otherPlayer = playerCollider.transform.parent.gameObject;

                if (!CheckForPlayer(otherPlayer)) {
                    continue;
                }

                if (!otherPlayer.Equals(gameObject)) {
                    PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                    if (otherPlayerLife) {
                        if (!otherPlayerLife.isAliveNetVar.Value && otherPlayerLife.isReportable) {
                            // otherPlayerLife.isReported = true;
                            Debug.Log("[ON REPORT] report player: " + otherPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
                            otherPlayer.GetComponent<PlayerStuff>().DestroyMeServerRpc();

                            VotingSelectionManager.Instance.SetPlayerServerRPC();
                            Invoke(nameof(StartConsultation), 1);
                            Invoke(nameof(StartEveluateConsultation),
                                21); // TODO: always check CanvasLogic:StartVoting countdown time
                            break;
                        }
                        else {
                            Debug.Log("[ON REPORT] isAlive or Reported");
                        }
                    }
                }
            }
        }
 
        public void StartConsultation()
        {
            TeleportManager.Instance.TeleportationServerRpc(Vector3.zero);
            CanvasLogic.Instance.StartVoting();
        }

        public void PerformKill() {
            if (!GetComponent<PlayerLife>().isImposterNetVar.Value) {
                return;
            }
            Collider2D[] colliders = CheckSorroundingArea();

            foreach (Collider2D playerCollider in colliders) {
                GameObject otherPlayer = playerCollider.transform.parent.gameObject;
                if (!CheckForPlayer(otherPlayer)) {
                    continue;
                }

                if (!otherPlayer.Equals(gameObject)) {
                    PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                    if (otherPlayerLife) {
                        if (!otherPlayerLife.isAliveNetVar.Value) {
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

        private void CheckForKill() {
            // Kill Imposter Only
            bool couldKill = false;


            Collider2D[] colliders = CheckSorroundingArea();

            foreach (Collider2D playerCollider in colliders) {
                GameObject otherPlayer = playerCollider.transform.parent.gameObject;
                if (!CheckForPlayer(otherPlayer)) {
                    continue;
                }

                if (!otherPlayer.Equals(gameObject)) {
                    PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                    if (otherPlayerLife) {
                        if (!otherPlayerLife.isAliveNetVar.Value) {
                            break;
                        }

                        couldKill = true;
                        //otherPlayerLife.Kill();
                    }
                    else {
                        Debug.LogWarning("[ON KILL] OtherPlayerLife is null? " + otherPlayerLife);
                    }

                    break;
                }
            }

            CanvasLogic.Instance.HighlightKillButton(couldKill);
        }

        private void CheckForReport() {
            // Report
            Collider2D[] colliders = CheckSorroundingArea();

            bool couldReport = false;

            foreach (Collider2D playerCollider in colliders) {
                GameObject otherPlayer = playerCollider.transform.parent.gameObject;

                if (!CheckForPlayer(otherPlayer)) {
                    continue;
                }

                if (!otherPlayer.Equals(gameObject)) {
                    PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                    if (otherPlayerLife) {
                        if (!otherPlayerLife.isAliveNetVar.Value && otherPlayerLife.isReportable) {
                            // otherPlayerLife.isReported = true;
                            // Could report
                            couldReport = true;
                            break;
                        }
                    }
                }
            }

            CanvasLogic.Instance.HighlightReportButton(couldReport);
        }

        private void StartEveluateConsultation() {
            VotingSelectionManager.Instance.EveluateConsultationServerRpc();
        }

        private bool CheckForPlayer(GameObject otherGameObject) {
            if (otherGameObject.CompareTag("Player")) {
                return true;
            }

            Debug.Log("[CheckForPlayer] Object is not Player?: " + otherGameObject);
            return false;
        }

        private Collider2D[] CheckSorroundingArea() {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), killRadius,
                LayerMask.GetMask("Player"));
            //Debug.Log("[CheckSorroundingArea] Collider size: " + colliders.Length);
            Array.Sort(colliders,
                (collider1, collider2) => (int) (UtilsUnity.getDistanceBetweenGameObjects(collider1.gameObject, gameObject) -
                                                 UtilsUnity.getDistanceBetweenGameObjects(collider2.gameObject, gameObject)));
            return colliders;
        }
        

    }
}