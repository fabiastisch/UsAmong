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
                Debug.LogWarning("CanvasLogic already exist.");
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

        public GameObject winScreen;

        public GameObject youreImpOrCrewMateScreen;
        public GameObject fullScreenOverlay;

        public GameObject killBtnObj;
        public GameObject reportBtnObj;
        public Image killCoolDownTime;

        public GameObject Coinbar;

        public GameObject chat;
        public GameObject openChatButton;

        public GameObject openSettingsButton;
        public GameObject settingsPanel;

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

        public void StartWinScreen(bool isImposterWin) {
            winScreen.SetActive(true);
            if (isImposterWin) {
                winScreen.GetComponent<TMP_Text>().text = "Imposters Win!";
            }
            else {
                winScreen.GetComponent<TMP_Text>().text = "Crewmates Win!";
            }
            SetFullScreenOverlay(true);
            Invoke(nameof(StopWinScreen), 5);
        }
        
        public void StopWinScreen() {
            winScreen.SetActive(false);
            SetFullScreenOverlay(false);
            SetStartButtonActive(true);
            killBtnObj.SetActive(true);
            openSettingsButton.SetActive(true);
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
            openChatButton.SetActive(false);
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

            if (NetUtils.GetLocalObject().GetComponent<PlayerLife>().isImposterNetVar.Value) {
                // if Imposter
                killBtnObj.SetActive(true);
            }

            if (active) {
                SetStartButtonActive(false);
                killBtnObj.SetActive(false);
                if (!NetUtils.GetLocalObject().GetComponent<PlayerLife>().isImposterNetVar.Value) {
                    // If in FullScreenOverlay or not Imposter
                    killBtnObj.SetActive(false);
                }
            }

            chat.SetActive(false);
            settingsPanel.SetActive(false);
            openSettingsButton.SetActive(false);
            openChatButton.SetActive(!active);
            Coinbar.SetActive(!active);
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

        public void SetCoolDownTimeValue(float leftCoolDownTime) {
            killCoolDownTime.fillAmount = leftCoolDownTime;
        }

        public void SetCoinBarValue(int health) {
            Coinbar.GetComponent<Slider>().value = health;
        }

        public void SetCoinbarMaxValue(int maxCoins) {
            Coinbar.GetComponent<Slider>().maxValue = maxCoins;
        }

        public void ChangeChatOpenStatus() {
            Debug.Log("[ChangeChatOpenStatus]");
            chat.SetActive(!chat.activeSelf);
        }

        public void OpenSettings() {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}