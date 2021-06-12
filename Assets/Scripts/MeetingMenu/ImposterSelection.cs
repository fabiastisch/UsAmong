using System.Collections;
using System.Collections.Generic;
using MLAPI.Spawning;
using UnityEngine;

public class ImposterSelection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public GameObject getLocalPlayer() {
        return NetworkSpawnManager.
        GetLocalPlayerObject().gameObject;;
    }
}
