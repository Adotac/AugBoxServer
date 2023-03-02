using FishNet.Object;
using UnityEngine;

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

    //void Update()
    //{
    //    if (!IsOwner) return;

    //    groundedPlayer = controller.isGrounded;
    //    if (groundedPlayer && playerVelocity.y < 0)
    //    {
    //        playerVelocity.y = 0f;
    //    }

    //    Vector3 move = new Vector3(input._horizontal, 0, input._vertical);
    //    controller.Move(move * Time.deltaTime * playerSpeed);

    //    if (move != Vector3.zero)
    //    {
    //        gameObject.transform.forward = move;
    //    }

    //    // Changes the height position of the player..
    //    if (input._jump && groundedPlayer)
    //    {
    //        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    //    }

    //    playerVelocity.y += gravityValue * Time.deltaTime;
    //    controller.Move(playerVelocity * Time.deltaTime);
    //}

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