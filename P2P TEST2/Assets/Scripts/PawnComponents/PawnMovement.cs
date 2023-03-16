using FishNet.Object;
using UnityEngine;

namespace MultiP2P
{
    public sealed class PawnMovement : NetworkBehaviour
    {
        private CharacterController controller;
        private PawnInput input;

        [SerializeField] private Vector3 playerVelocity;
        [SerializeField] private bool groundedPlayer;
        [SerializeField] private float playerSpeed = 10.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravityValue = 9.81f; 

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            input = GetComponent<PawnInput>();
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!IsOwner) return;

            Vector3 desiredVelocity = Vector3.ClampMagnitude(((transform.forward * input._vertical) + (transform.right * input._horizontal)) * playerSpeed, playerSpeed);

            playerVelocity.x = desiredVelocity.x;
            playerVelocity.z = desiredVelocity.z;

            if (controller.isGrounded)
            {
                playerVelocity.y = 0.0f;

                if (input._jump)
                {
                    playerVelocity.y = jumpHeight;
                }
            }
            else
            {
                playerVelocity.y += Physics.gravity.y * gravityValue * Time.deltaTime;
            }

            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
}