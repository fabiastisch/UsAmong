using UnityEngine;

namespace Utils {
    public static class UtilsUnity {
        public static float getDistanceBetweenGameObjects(GameObject g1, GameObject g2) {
            return Vector3.Magnitude(g1.transform.position - g2.transform.position);
        }

        public static int GetRandomInt(int min, int max) {
            return Random.Range(min, max + 1);
        }
        public static int GetRandomInt(int max) {
            return Random.Range(0, max + 1);
        }
        
    }
}