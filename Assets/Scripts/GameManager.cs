using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
            SubmitNewPosition();
        }

        GUILayout.EndArea();
    }

    public void StartButtons()
    {
        if (GUILayout.Button("Start Host"))
            NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Start Server"))
            NetworkManager.Singleton.StartServer();
        if (GUILayout.Button("Start Client"))
            NetworkManager.Singleton.StartClient();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        GUILayout.Label("Mode: " + mode);
    }

    static void SubmitNewPosition()
    {
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Move"))
        {
            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    var otherPlayerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
                    var otherPlayer = otherPlayerObject.GetComponent<Player>();
                    otherPlayer.Move();
                }
            }
            else
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<Player>();
                player.Move();
            }
        }
    }
}
