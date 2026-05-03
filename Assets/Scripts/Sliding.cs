    using UnityEngine;
    using System.Collections;

    public class Sliding : MonoBehaviour
    {
        
        [SerializeField] Movement movement;

        [SerializeField] WallRebound wallRebound;

        [SerializeField] Dash dash;
    
        [SerializeField] float slideVel;

        [SerializeField] float slideDuration = 0.5f;
    
        [SerializeField] Rigidbody rb;
        [SerializeField] Transform headTarget;
        
        [SerializeField] Transform model;

        private Vector3 normalScale = new Vector3(1f, 1f, 1f);
        private Vector3 slideScale = new Vector3(1f, 0.3f, 1f);

        private bool isSliding = false;

        public bool IsSliding => isSliding;

        void Awake()
        {
            if (movement == null)
                movement = GetComponent<Movement>();

            if (wallRebound == null)
            {
                wallRebound = GetComponent<WallRebound>();
            }

            if (dash == null)
            {
                dash = GetComponent<Dash>();
            }
        }

        bool CanStandUp()
        {
            // Cast upward from the player's position
            float checkHeight = 2f; // height difference between slideScale and normalScale
            float radius = 0.4f;    // adjust to match your player width

            return !Physics.SphereCast(
                transform.position,
                radius,
                Vector3.up,
                out RaycastHit hit,
                checkHeight,
                LayerMask.GetMask("Default") // or your environment layer
            );
        }

        public void Slide(PlayerControls controls)
        {
            if (controls.Player.Slide.triggered && !IsSliding && movement.IsGrounded && !wallRebound.onwall && !dash.IsDashing)
            {
                StartCoroutine(SlideRoutine());
            }
        }

        IEnumerator SlideRoutine()
        {
            Debug.Log("[SLIDE] SlideRoutine started");

            isSliding = true;

            model.localScale = slideScale;

            rb.linearVelocity += transform.forward * slideVel;
            Debug.Log("[SLIDE] Slide burst velocity: " + rb.linearVelocity);

            yield return new WaitForSeconds(slideDuration);

            while (!CanStandUp())
            {
                rb.linearVelocity = transform.forward * slideVel * 0.5f;
                yield return null;
            }

            model.localScale = normalScale;

            isSliding = false;

            Debug.Log("[SLIDE] SlideRoutine ended");
        }
    }
