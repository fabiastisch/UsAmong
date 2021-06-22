using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Created");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        CoinManager.Instance.minimizeRemainingCoins();
    }
    
}
