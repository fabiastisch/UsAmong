using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Teleport
{
    public class Teleport : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector2 random = new Vector2(Random.Range(-160f, 40f), Random.Range(-20f, -140f));
            while (Physics2D.OverlapCircleAll(random, 3f).Length > 0)
            { 
                random = new Vector2(Random.Range(-160f, 40f), Random.Range(-20f, -140f));
            }

            other.gameObject.transform.position = random;
        }
    }
}