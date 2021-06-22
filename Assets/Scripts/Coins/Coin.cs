using UnityEngine;

namespace Coins {
    public class Coin : MonoBehaviour {
        void Start() {
            //Debug.Log("[Coin] Created");
        }
    
        private void OnTriggerEnter2D(Collider2D other) {
            CoinManager.Instance.minimizeRemainingCoins(gameObject);
        }
    
    }
}