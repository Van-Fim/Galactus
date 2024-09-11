using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetManager : NetworkManager
{
    bool onlineSceneLoaded;
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == onlineScene)
            StartCoroutine(InitStartContent());
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
        GameObject player = Instantiate(playerPrefab, start);
        player.transform.SetParent(null);
        NetSpaceObject netPlayer = player.GetComponent<NetSpaceObject>();
        netPlayer.isPlayer = true;
        yield return new WaitForEndOfFrame();
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    IEnumerator InitStartContent()
    {
        GameManager.singleton.LoadContent();
        onlineSceneLoaded = true;
        yield return new WaitForEndOfFrame();
    }
}
