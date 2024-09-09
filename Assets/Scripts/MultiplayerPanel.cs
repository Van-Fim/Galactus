using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MultiplayerPanel : MonoBehaviour
{
    public static MultiplayerPanel singleton;
    public void Awake(){
        singleton = this;
    }
    public void StartServer()
    {
        NetworkManager.singleton.StartServer();
        singleton.gameObject.SetActive(false);
    }
    public void StartHost()
    {
        NetworkManager.singleton.StartHost();
        singleton.gameObject.SetActive(false);
    }
    public void StartClient()
    {
        NetworkManager.singleton.StartClient();
        singleton.gameObject.SetActive(false);
    }
}
