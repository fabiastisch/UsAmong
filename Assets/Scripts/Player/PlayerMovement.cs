using MLAPI;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(NetworkObject), typeof(Rigidbody2D))]
    public class PlayerMovement : NetworkBehaviour {
        public float speed = 10;
        
        private Rigidbody2D rb;

        private Animator anim;
        private int leftHash = Animator.StringToHash("Left");
        private int rightHash = Animator.StringToHash("Right");

        // Start is called before the first frame update
        void Start() {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update() {
            if (!IsLocalPlayer) {
                return;
            }
            float horizontal = Input.GetAxis("Horizontal"); // d = 1, a = -1
            float vertical = Input.GetAxis("Vertical"); // w = 1 , s = -1 

            Vector2 velocity = new Vector2(horizontal, vertical);
            rb.velocity = velocity * speed;

            if (horizontal > 0) { // show right
                anim.SetTrigger(rightHash);
            }

            if (horizontal < 0) { // show left
                anim.SetTrigger(leftHash);
            }
        }
    }
}