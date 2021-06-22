using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    private bool isAvailabe = true;
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Created");
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Destroy Coin"); // TODO: called twice
        CoinManager.Instance.minimizeRemainingCoins(gameObject);
    }

    private void OnDestroy() {
        /*
        if (isAvailabe) {
            
            isAvailabe = false;
        }
    */
    }
}