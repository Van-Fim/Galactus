using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class NetManager : NetworkManager
{
    bool onlineSceneLoaded;
    public void Start()
    {
        GameContentManager.InitContent();
        GameContentManager.LoadPrefabs();
        CameraManager.Init();
        CanvasManager.Init();
        
        CameraManager.SwitchByCode(0);

        ClientManager.accountLogin = "AccountLogin01";

        NetClient cl = new NetClient();
        cl.password = NetClient.StrToMD5("password");
        cl.serverIdentity = cl.GenerateServerIdentity(false);
        NetClient.localClient = cl;
    }
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
        NetClient.playerObject = player.gameObject;

        yield return new WaitForEndOfFrame();
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    IEnumerator InitStartContent()
    {
        SpaceManager.singleton = Instantiate(GameContentManager.spaceManagerPrefab);
        NetworkServer.Spawn(SpaceManager.singleton.gameObject);
        
        SpaceObjectNetManager.singleton = Instantiate(GameContentManager.spaceObjectNetManagerPrefab);
        NetworkServer.Spawn(SpaceObjectNetManager.singleton.gameObject);

        ClientManager.singleton = Instantiate(GameContentManager.clientManagerPrefab);
        NetworkServer.Spawn(ClientManager.singleton.gameObject);
        
        onlineSceneLoaded = true;
        yield return 1;
    }
}
