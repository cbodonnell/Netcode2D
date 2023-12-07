using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using System;

public class PredictiveServerPlayerMovement : NetworkBehaviour
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

    private int tick = 0;
    private float tickRate = 1f / 60f;
    private float tickDeltaTime = 0f;

    private const int buffer = 1024;

    private HandleStates.InputState[] _inputStates = new HandleStates.InputState[buffer];
    private HandleStates.TransformStateRW[] _transformStates = new HandleStates.TransformStateRW[buffer];

    private NetworkVariable<HandleStates.TransformStateRW> currentServerTransformState = new();

    private HandleStates.TransformStateRW previousTransformState;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = new MyPlayerInput();
        playerInput.Enable();
    }

    private void OnServerStateChanged(HandleStates.TransformStateRW previousValue, HandleStates.TransformStateRW newValue)
    {
        previousTransformState = previousValue;
    }

    private void OnEnable()
    {
        currentServerTransformState.OnValueChanged += OnServerStateChanged;
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

    public void ProcessLocalPlayerMovement(Vector2 _moveInput)
    {
        tickDeltaTime += Time.deltaTime;

        if (tickDeltaTime > tickRate)
        {
            int bufferIndex = tick % buffer;

            MovePlayerWithServerTickServerRpc(tick, _moveInput);
            Move(_moveInput);

            HandleStates.InputState inputState = new()
            {
                tick = tick,
                moveInput = _moveInput
            };

            HandleStates.TransformStateRW transformState = new()
            {
                tick = tick,
                position = playerTransform.position,
                isMoving = true
            };

            _inputStates[bufferIndex] = inputState;
            _transformStates[bufferIndex] = transformState;

            tickDeltaTime -= tickRate;
            if (tick == buffer)
            {
                tick = 0;
            }
            else
            {
                tick++;
            }
        }
    }

    [ServerRpc]
    private void MovePlayerWithServerTickServerRpc(int tick, Vector2 moveInput)
    {
            Move(moveInput);

            HandleStates.TransformStateRW transformState = new()
            {
                tick = tick,
                position = playerTransform.position,
                isMoving = true
            };

            previousTransformState = currentServerTransformState.Value;
            currentServerTransformState.Value = transformState;
    }

    public void SimulateOtherPlayerMovement()
    {
        tickDeltaTime += Time.deltaTime;

        if (tickDeltaTime > tickRate)
        {
            /// TODO: fix null reference exception
            if (currentServerTransformState.Value.isMoving)
            {
                transform.position = currentServerTransformState.Value.position;
            }

            tickDeltaTime -= tickRate;

            if (tick == buffer)
            {
                tick = 0;
            }
            else
            {
                tick++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newInput = playerInput.Player.Movement.ReadValue<Vector2>();

        if (IsClient && IsLocalPlayer)
        {
            ProcessLocalPlayerMovement(newInput);
        }
        else
        {
            SimulateOtherPlayerMovement();
        }
    }

    private void Move(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, input.y, 0);
        Vector2 v = move * playerSpeed;
        Vector3 motion = v * tickRate;
        cc.Move(motion);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 newInput)
    {
        currentInput = newInput;
    }
}
