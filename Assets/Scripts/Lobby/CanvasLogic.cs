using System;
using TMPro;
using UnityEngine;

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

        private float countDownTimeLeft = 0f;
        private bool isCountDownActive = false;


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
    }
}