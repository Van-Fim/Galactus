using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class NetSystem : NetworkManager
{
    bool onlineSceneLoaded;

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == onlineScene)
        {
            StartCoroutine(InitStartContent());
        }
    }
    public override void OnClientDisconnect()
    {

    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (ServerDataManager.singleton != null)
        {
            //ServerDataManager.singleton.SaveServerData();
        }
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        if (conn.identity == null)
            StartCoroutine(AddPlayerDelayed(conn));
    }
    IEnumerator AddPlayerDelayed(NetworkConnectionToClient conn)
    {
        while (!onlineSceneLoaded)
            yield return null;
        Transform start = GetStartPosition();
        NetClient clientPrefab = playerPrefab.GetComponent<NetClient>(); 
        NetClient player = Instantiate<NetClient>(clientPrefab, start);
        player.transform.SetParent(null);
        yield return new WaitForEndOfFrame();
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
    }

    IEnumerator InitStartContent()
    {
        ServerDataManager.Init();
        NetworkServer.Spawn(ServerDataManager.singleton.gameObject);
        ServerDataManager.singleton.ServerData.LoadServerData();
        ServerDataManager.singleton.ServerData.gameStarts = GameStartManager.LoadGameStarts();
        onlineSceneLoaded = true;
        yield return 1;
    }
}