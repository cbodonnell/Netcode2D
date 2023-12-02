using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Vector2> Position = new NetworkVariable<Vector2>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
            transform.position = randomPosition;
            Position.Value = randomPosition;
        }
        else
        {
            SubmitPositionServerRpc();
        }
    }


    [ServerRpc]
    void SubmitPositionServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
    }

    void Update()
    {
        transform.position = Position.Value;
    }
}
