using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using System;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private float playerSpeed = 5f;
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private CinemachineVirtualCamera vc;
    [SerializeField]
    private AudioListener listener;

    public CharacterController cc;
    private MyPlayerInput playerInput;

    private Vector2 currentInput;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = new MyPlayerInput();
        playerInput.Enable();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
        }
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
        Vector3 move = new Vector3(input.x, input.y, 0);
        Vector2 v = move * playerSpeed;
        Debug.Log("Moving with velocity: " + v);
        Vector3 motion = v * Time.deltaTime;
        cc.Move(motion);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 newInput)
    {
        currentInput = newInput;
    }
}
