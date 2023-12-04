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

    private Vector2 currentInput;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = new MyPlayerInput();
        playerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer && IsLocalPlayer)
        {
            currentInput = playerInput.Player.Movement.ReadValue<Vector2>();
        }
        else if (IsClient && IsLocalPlayer)
        {
            Vector2 newInput = playerInput.Player.Movement.ReadValue<Vector2>();
            MoveServerRpc(newInput);
        }

        if (IsServer)
        {
            Move(currentInput);
        }
    }

    private void Move(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);
        cc.Move(move * playerSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 newInput)
    {
        currentInput = newInput;
    }
}
