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
        Client client = player.GetComponent<Client>();
        client.login += "_" + conn.connectionId;
        client.Init();

        yield return new WaitForEndOfFrame();
        NetworkServer.AddPlayerForConnection(conn, player);
        ClientManager.singleton.clientIds.Add(client.netId);
    }

    IEnumerator InitStartContent()
    {
        SPObjectManager.Init();
        NetworkServer.Spawn(SPObjectManager.singleton.gameObject);
        ClientManager.Init();
        NetworkServer.Spawn(ClientManager.singleton.gameObject);
        onlineSceneLoaded = true;

        SpaceManager.Init();
        SpaceManager.singleton.Load();

        for (int i = 0; i < SpaceManager.starSystems.Count; i++)
        {
            StarSystem system = SpaceManager.starSystems[i];
            system.LoadAsteroids(90, 100, 200);
        }

        yield return 1;
    }
}