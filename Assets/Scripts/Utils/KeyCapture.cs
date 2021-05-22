using System;
using UnityEngine;

namespace Utils {
    public class KeyCapture {
        public KeyCapture() {
            Debug.Log("New KeyCapture");
        }

        bool captureKey = true;

        public void Update() {
            Debug.Log("VAR.Update");
            ReassignKey();
        }


        void ReassignKey() {
            if (!captureKey) return;

            if (Input.inputString.Length <= 0) return;

            Debug.Log(Input.inputString);
            captureKey = false;
        }
    }
}