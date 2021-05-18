using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using Utils;

// Script requires the GameObject to have a NetworkObject component
[RequireComponent(typeof(NetworkObject))]
public class LobbyManager : NetworkBehaviour {
    public GameObject playerObject;

    private GameObject player;

    // Start is called before the first frame update
    void Start() {
        if (!NetUtils.IsConnected()) {
            return;
        }

        /*if (IsClient) {
            Debug.Log("SpawnMe ServerRPS -. ");
            SpawnMeServerRpc(NetworkManager.Singleton.LocalClientId);
        }else {
            Vector3 random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            player = Instantiate(playerObject, random, Quaternion.identity);
        }*/
    }

    // Update is called once per frame
    void Update() {
    }

    [ServerRpc]
    private void SpawnMeServerRpc(ulong clientId) {
        Debug.Log("Server RPC");
        Vector3 random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        player = Instantiate(playerObject, random, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyMeServerRpc() {
        player.GetComponent<NetworkObject>().Despawn(true);
    }
}