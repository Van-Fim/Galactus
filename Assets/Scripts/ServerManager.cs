using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStartedHandler;
    }

    private void OnServerStartedHandler()
    {
        Debug.Log("Server has started and is now listening for connections.");
    }
}
