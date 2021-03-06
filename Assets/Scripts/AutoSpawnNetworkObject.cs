using MLAPI;
using UnityEngine;
using Utils;

[RequireComponent(typeof(NetworkObject))]
public class AutoSpawnNetworkObject : NetworkBehaviour {
    // Start is called before the first frame update
    void Start() {
        if (!NetUtils.IsConnected()) {
            return;
        }
        if (NetUtils.IsServer()) {
            GetComponent<NetworkObject>().Spawn();
        }
    }
    
}