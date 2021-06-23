using System;
using MLAPI;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Lobby {
    public class CanvasLogic : MonoBehaviour {
        #region SingletonPattern

        private static CanvasLogic instance;

        public static CanvasLogic Instance {
            get => instance;
        }

        public static event Action OnSingletonReady;

        private void Awake() {
            if (instance == null) {
                instance = this;
                OnSingletonReady?.Invoke();
            }
            else if (instance != this) {
                Debug.LogWarning("StartButton already exist.");
                Destroy(gameObject);
            }
        }

        #endregion

        public Transform start;

        public GameObject startButton;

        public GameObject votingObj;

        public GameObject votingResultObj;

        public GameObject countdownObj;

        public GameObject playerWinScreen;
        public GameObject imposterWinScreen;


        public GameObject killBtnObj;
        public GameObject reportBtnObj;

        private float countDownTimeLeft = 0f;
        private bool isCountDownActive = false;
        
        private void Start() {
            killBtnObj.GetComponent<Button>().onClick.AddListener(() => {
                NetUtils.GetLocalObject().GetComponent<PlayerControls>().PerformKill();
            });
            reportBtnObj.GetComponent<Button>().onClick.AddListener(() => {
                NetUtils.GetLocalObject().GetComponent<PlayerControls>().PerformReport();
            });
            NetworkObject networkObject = NetUtils.GetLocalObject();
            if (!networkObject) {
                return;
            }
            networkObject.GetComponent<PlayerLife>().isAliveNetVar.OnValueChanged += (value, newValue) => {
                if (!newValue) {
                    if (killBtnObj.activeSelf) {
                        killBtnObj.SetActive(false);
                    }

                    if (reportBtnObj.activeSelf) {
                        reportBtnObj.SetActive(false);
                    }
                }
                else {
                    if (!killBtnObj.activeSelf) {
                        killBtnObj.SetActive(true);
                    }

                    if (!reportBtnObj.activeSelf) {
                        reportBtnObj.SetActive(true);
                    }
                }
            };
        }

        public void OnStartButtonClicked() {
            Debug.Log("Start Button CLICKED");
            //SceneLoaderManager.LoadGame();
            LobbyManager.Singleton.StartGameServerRpc(start.position);
            CoinManager coinmanager = CoinManager.Instance;
            coinmanager.SetPlayerServerRPC();
            coinmanager.DetermineNumberOfCoinsServerRPC();
            //LobbyManager.Singleton.getLocalPlayer().transform.position = start.position;
        }

        public void SetStartButtonActive(bool value) {
            startButton.SetActive(value);
        }

        public void StartVoting() {
            votingObj.SetActive(true);
            StartCountdown(20); // TODO: always update Countdown with voting time 
        }

        public void StopVoting() {
            votingObj.SetActive(false);
            votingResultObj.SetActive(true);
            StopCountdown();
            Invoke(nameof(StopShowingResult), 4);
        }

        public void StartPlayerWinScreen()
        {
            imposterWinScreen.SetActive(true);
            Invoke(nameof(StopPlayerWinScreen), 10);
        }
        
        public void StopPlayerWinScreen()
        {
            imposterWinScreen.SetActive(true);
        }
        
        public void StartImposterWinScreen()
        {
            imposterWinScreen.SetActive(true);
            Invoke(nameof(StopImposterWinScreen), 10);
        }
        
        public void StopImposterWinScreen()
        {
            imposterWinScreen.SetActive(false);
        }

        public void StopShowingResult() {
            votingResultObj.SetActive(false);
        }

        public void StartCountdown(int coundowntime) {
            countdownObj.SetActive(true);
            if (startButton.activeSelf) {
                SetStartButtonActive(false);
            }

            countDownTimeLeft = coundowntime;
            isCountDownActive = true;
        }

        public void StopCountdown() {
            isCountDownActive = false;
            countdownObj.SetActive(false);
        }

        private void Update() {
            if (isCountDownActive) {
                countDownTimeLeft -= Time.deltaTime;
                countdownObj.GetComponent<TMP_Text>().text = ((int) countDownTimeLeft % 60 + 1).ToString();
            }
        }

        public void HighlightKillButton(bool status) {
            if (status) {
                var tmpColor = new Color(255, 255, 255);
                tmpColor.a = 1f;
                killBtnObj.GetComponent<Image>().color = tmpColor;
            }
            else {
                var tmpColor = new Color(128, 128, 128);
                tmpColor.a = 0.5f;
                killBtnObj.GetComponent<Image>().color = tmpColor;
            }
        }

        public void HighlightReportButton(bool status) {
            if (status) {
                var tmpColor = new Color(255, 255, 255);
                tmpColor.a = 1f;
                reportBtnObj.GetComponent<Image>().color = tmpColor;
            }
            else {
                var tmpColor = new Color(128, 128, 128);
                tmpColor.a = 0.5f;
                reportBtnObj.GetComponent<Image>().color = tmpColor;
            }
        }
    }
}