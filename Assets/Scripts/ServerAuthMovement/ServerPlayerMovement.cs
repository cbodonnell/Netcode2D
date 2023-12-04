using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private float playerSpeed = 5f;
    [SerializeField]
    private Transform playerTransform;

    public CharacterController cc;
    private MyPlayerInput playerInput;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = new MyPlayerInput();
        playerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = playerInput.Player.Movement.ReadValue<Vector2>();

        if (IsServer && IsLocalPlayer)
        {
            Move(input);
        }
        else if (IsClient && IsLocalPlayer)
        {
            MoveServerRpc(input);
        }
    }

    private void Move(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);
        cc.Move(move * playerSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 input)
    {
        Move(input);
    }
}
