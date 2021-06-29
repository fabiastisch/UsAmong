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

        public bool inGame = false;

        public Transform start;

        public GameObject startButton;

        public GameObject votingObj;

        public GameObject votingResultObj;

        public GameObject countdownObj;

        public GameObject playerWinScreen;
        public GameObject imposterWinScreen;

        public GameObject youreImpOrCrewMateScreen;
        public GameObject fullScreenOverlay;

        public GameObject killBtnObj;
        public GameObject reportBtnObj;
        public Image killCoolDownTime;

        public GameObject Coinbar;
        
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
            
            killCoolDownTime.fillAmount = 0;
        }

        public void OnStartButtonClicked() {
            inGame = true;
            LobbyManager.Singleton.StartGameServerRpc();
            LobbyManager.Singleton.DetermineNumberOfLivingCrewmatesServerRPC();
            CoinManager.Instance.DetermineNumberOfCoinsServerRPC();
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

        public void StartCrewMatesWinScreen() {
            playerWinScreen.SetActive(true);
            Invoke(nameof(StopPlayerWinScreen), 10);
        }

        public void StopPlayerWinScreen() {
            playerWinScreen.SetActive(false);
        }

        public void StartImposterWinScreen() {
            imposterWinScreen.SetActive(true);
            Invoke(nameof(StopImposterWinScreen), 10);
        }

        public void StopImposterWinScreen() {
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

        public void SetYoureImpOrCrewMate(float seconds) {
            youreImpOrCrewMateScreen.SetActive(true);
            SetFullScreenOverlay(true);
            bool isImposter = NetUtils.GetLocalObject().GetComponent<PlayerLife>().isImposterNetVar.Value;
            TMP_Text text = youreImpOrCrewMateScreen.GetComponent<TMP_Text>();
            if (isImposter) {
                text.text = "You are Imposter.";
            }
            else {
                text.text = "You are Crewmate.";
            }
            Invoke(nameof(HideActiveYoureImpOrCrew), seconds);
        }

        private void HideActiveYoureImpOrCrew() {
            youreImpOrCrewMateScreen.SetActive(false);
            SetFullScreenOverlay(false);
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

        private void SetFullScreenOverlay(bool active) {
            fullScreenOverlay.SetActive(active);
            if (!NetUtils.GetLocalObject().GetComponent<PlayerLife>().isImposterNetVar.Value) {
                killBtnObj.SetActive(false);
            }

            reportBtnObj.SetActive(!active);
        }

        public void HighlightReportButton(bool active) {
            if (active) {
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
        
        public void SetCoolDownTimeValue(float leftCoolDownTime)
        {
            killCoolDownTime.fillAmount = leftCoolDownTime;
        }

        public void SetCoinBarValue(int health)
        {
            Coinbar.GetComponent<Slider>().value = health;
        }

        public void SetCoinbarMaxValue(int maxCoins)
        {
            Coinbar.GetComponent<Slider>().maxValue = maxCoins;
        }
    }
}