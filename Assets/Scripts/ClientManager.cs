using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientManager : NetworkBehaviour
{
    public static ClientManager singleton;
    public static string accountLogin = "Login01";
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void OnStartClient()
    {
        singleton = this;
    }
}
