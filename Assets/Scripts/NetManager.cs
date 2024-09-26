using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetManager : NetworkManager
{
    bool onlineSceneLoaded;
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<LoginData>(OnLoginMessageReceived);
    }
    void OnLoginMessageReceived(NetworkConnection conn, LoginData msg)
    {
        ValidateLogin(msg);
    }
    public void ValidateLogin(LoginData msg)
    {
        GameStartData gameStartData = GameStartManager.LoadGameStart("Start01");
        gameStartData.spaceObjectDatas = SpaceObjectManager.ReadSpaceContent("Start01", "start");
        SpaceObjectData plyData = gameStartData.spaceObjectDatas.Find(x => x.isPlayerControll == true);
        plyData.sendSuccessfull = true;
        msg.netId.sp = plyData;
        SpaceObject obj = SpaceObject.Create(plyData, msg.netId.gameObject);
        obj.Init();
        obj.LoadHardpoints();
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
        NetSpaceObject netPlayer = player.GetComponent<NetSpaceObject>();
        netPlayer.isPlayer = true;
        yield return new WaitForEndOfFrame();
        NetworkServer.AddPlayerForConnection(conn, player);
        NetClientManager.singleton.netSpaceObjects.Add(netPlayer);
        netPlayer.StartClient();
    }

    IEnumerator InitStartContent()
    {
        LocalClient.isServer = true;
        GameManager.singleton.LoadContent();
        onlineSceneLoaded = true;
        NetClientManager netClientManager = GamePrefabsManager.LoadPrefab<NetClientManager>("NetClientManagerPrefab");
        netClientManager = Instantiate(netClientManager);
        NetworkServer.Spawn(netClientManager.gameObject);
        yield return new WaitForEndOfFrame();
    }
}
