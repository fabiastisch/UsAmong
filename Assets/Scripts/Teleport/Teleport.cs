using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Teleport
{
    public class Teleport : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector3 random = new Vector3(Random.Range(-160f, 40f), Random.Range(-20f, -140f), 0);
            TeleportManager.Instance.TeleportationServerRpc(random,other.gameObject);        
        }
    }
}