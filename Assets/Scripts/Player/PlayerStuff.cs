using System;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerStuff : NetworkBehaviour {
        public TMP_Text playerNameTMP;

        [SerializeField]
        private NetworkVariableString playerName = new NetworkVariableString(NetUtils.Everyone);

        public NetworkVariableString PlayerName => playerName;
        
        private void OnEnable() {
            PlayerName.OnValueChanged += OnPlayerNameChanged;
        }

        private void OnDisable() {
            PlayerName.OnValueChanged -= OnPlayerNameChanged;
        }

        private void OnPlayerNameChanged(string previousvalue, string newvalue) {
            playerNameTMP.text = newvalue;
        }

    }
}