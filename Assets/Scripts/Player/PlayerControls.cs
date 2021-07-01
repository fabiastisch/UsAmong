using System;
using System.Collections;
using Lobby;
using MLAPI;
using MLAPI.NetworkVariable;
using Teleport;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerControls : NetworkBehaviour {
        public float killRadius = 3;
        private PlayerLife _playerLife;
        public bool killCoolDownActive = false;
        public float coolDownTimePercentage = 0f;


        // Start is called before the first frame update
        void Start() {
            _playerLife = GetComponent<PlayerLife>();
        }

        // Update is called once per frame
        void Update() {
            if (!IsLocalPlayer) {
                return;
            }
            
            if (CanvasLogic.Instance.chat.activeSelf) {
                // If Chat is active
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
                    if (!killCoolDownActive) {
                        // Kill Imposter Only
                        PerformKill();
                    }
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

        private void ActivateCoolDown() {
            killCoolDownActive = true;
            coolDownTimePercentage = 1;
            StartCoroutine(nameof(MinimizeCoolDownTime));
            Invoke(nameof(ReactivateKill), 20);
        }

        public IEnumerator MinimizeCoolDownTime() {
            while (coolDownTimePercentage >= 0) {
                coolDownTimePercentage -= 0.05f;
                CanvasLogic.Instance.SetCoolDownTimeValue(coolDownTimePercentage);
                yield return new WaitForSeconds(1);
            }
        }

        public void ReactivateKill() {
            killCoolDownActive = false;
            coolDownTimePercentage = 0;
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
                            Debug.Log("[ON REPORT] report player: " +
                                      otherPlayer.GetComponent<PlayerStuff>().PlayerName.Value);
                            otherPlayer.GetComponent<PlayerLife>().Kill();

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

        public void StartConsultation() {
            TeleportManager.Instance.TeleportationServerRpc(Vector3.zero);
            CanvasLogic.Instance.StartVoting();
        }

        public void PerformKill() {
            if (!GetComponent<PlayerLife>().isImposterNetVar.Value) {
                return;
            }
            ActivateCoolDown();

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

            if (!killCoolDownActive) {
                Collider2D[] colliders = CheckSorroundingArea();

                foreach (Collider2D playerCollider in colliders) {
                    GameObject otherPlayer = playerCollider.transform.parent.gameObject;
                    if (!CheckForPlayer(otherPlayer)) {
                        continue;
                    }

                    if (!otherPlayer.Equals(gameObject)) {
                        PlayerLife otherPlayerLife = otherPlayer.GetComponent<PlayerLife>();
                        if (otherPlayerLife) {
                            // Debug.Log("[CHECKFORKILL]" + otherPlayerLife.isAliveNetVar.Value);
                            if (!otherPlayerLife.isAliveNetVar.Value) {
                                break;
                            }

                            couldKill = true;
                        }
                        else {
                            Debug.LogWarning("[ON KILL] OtherPlayerLife is null? " + otherPlayerLife);
                        }

                        break;
                    }
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
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y),
                killRadius,
                LayerMask.GetMask("Player"));
            //Debug.Log("[CheckSorroundingArea] Collider size: " + colliders.Length);
            Array.Sort(colliders,
                (collider1, collider2) =>
                    (int) (UtilsUnity.getDistanceBetweenGameObjects(collider1.gameObject, gameObject) -
                           UtilsUnity.getDistanceBetweenGameObjects(collider2.gameObject, gameObject)));
            return colliders;
        }
    }
}