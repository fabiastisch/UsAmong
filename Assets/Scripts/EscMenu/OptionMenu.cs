using TMPro;
using UnityEngine;
using Utils;

namespace EscMenu {
    public class OptionMenu : MonoBehaviour {
        public TMP_Text textText;
        private KeyCapture KeyCapture;

        public void TestButtonClicked() {
            KeyCapture = new KeyCapture();
        }

        private void Update() {
            KeyCapture?.Update();
        }
    }
}