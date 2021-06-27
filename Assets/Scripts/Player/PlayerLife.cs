using System.Collections;
using Lobby;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;
using Utils;

namespace Player {
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerLife : NetworkBehaviour
    {
        public NetworkVariableBool isAliveNetVar = new NetworkVariableBool(NetUtils.Everyone, true);
        public bool isReportable = false;
        public NetworkVariableBool isImposterNetVar = new NetworkVariableBool(NetUtils.Everyone, false);
        private ArrayList deadBodys = new ArrayList();

        private void Start()
        {
            // Change Color if Imposter
            isImposterNetVar.OnValueChanged += (value, newValue) =>
            {
                GameObject local = NetUtils.GetLocalObject().gameObject;
                if (local.GetComponent<PlayerLife>().isImposterNetVar.Value)
                {
                    if (newValue)
                    {
                        GetComponent<PlayerStuff>().playerNameTMP.color = Color.red;
                    }
                }

                if (!newValue)
                {
                    GetComponent<PlayerStuff>().playerNameTMP.color = Color.white;
                }
            };
        }

        public void Kill()
        {
            isAliveNetVar.Value = false;
            KillServerRPC(GetComponent<NetworkObject>().OwnerClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void KillServerRPC(ulong killedPlayerId)
        {
            NetworkObject netObj = NetworkManager.ConnectedClients[killedPlayerId].PlayerObject;
            netObj.GetComponent<PlayerStuff>().PlayerName.Value += "[DEAD]";
            GameObject deadBody = LobbyManager.Singleton.deadPlayerObject;
            GameObject instanceDeadBody = Instantiate(deadBody, transform.position, Quaternion.identity);
            instanceDeadBody.GetComponent<NetworkObject>().Spawn();
            deadBodys.Add(instanceDeadBody);

            updateAmountOfLivingBeings(netObj);
            DetermineVictory();
        }

        public void updateAmountOfLivingBeings(NetworkObject client)
        {
            LobbyManager.Singleton.DetermineNumberOfLivingCrewmatesServerRPC();
            if (client.GetComponent<PlayerLife>().isImposterNetVar.Value)
            {
                LobbyManager.Singleton.MinimizeImposterNumber();
            }
        }

        public void DetermineVictory()
        {
            LobbyManager lobbyManager = LobbyManager.Singleton;

            if (lobbyManager.livingCrewMates.Value <= lobbyManager.impostersCount)
            {
                CanvasLogic.Instance.StartImposterWinScreen();
                LobbyManager.Singleton.ResetGameServerRpc();
            }
            else if (lobbyManager.impostersCount == 0)
            {
                CanvasLogic.Instance.StartCrewMatesWinScreen();
                LobbyManager.Singleton.ResetGameServerRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void DestroyDeadBodyServerRPC()
        {
            foreach (GameObject deadBody in deadBodys)
            {
                Destroy(deadBody);
            }
        }
    }
}