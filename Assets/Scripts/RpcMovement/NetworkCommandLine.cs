using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkCommandLine : MonoBehaviour
{
    private NetworkManager networkManager;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor) return;

        Dictionary<string, string> args = GetCommandLineArgs();
        if (args.ContainsKey("server"))
        {
            Console.WriteLine("Starting server");
            networkManager.StartServer();
        }
        else if (args.ContainsKey("client"))
        {
            Console.WriteLine("Starting client");
            networkManager.StartClient();   
        }
        else if (args.ContainsKey("host"))
        {
            Console.WriteLine("Starting host");
            networkManager.StartHost();
        }
    }

    private Dictionary<string, string> GetCommandLineArgs()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        string[] commandLineArgs = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < commandLineArgs.Length; i++)
        {
            var arg = commandLineArgs[i].ToLower();
            if (arg.StartsWith("-"))
            {
                arg = arg.Substring(1);
                if (i + 1 < commandLineArgs.Length && !commandLineArgs[i + 1].StartsWith("-"))
                {
                    args.Add(arg, commandLineArgs[i + 1]);
                    i++;
                }
                else
                {
                    args.Add(arg, "");
                }
            }
        }
        return args;
    }
}
