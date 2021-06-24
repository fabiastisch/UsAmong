using UnityEngine;

namespace Coins {
    public class Coin : MonoBehaviour {

        private void OnTriggerEnter2D(Collider2D other) {
            GameObject player = other.transform.parent.gameObject;
            CoinManager.Instance.determineRemainingCoins(gameObject, player);
        }
    
    }
}