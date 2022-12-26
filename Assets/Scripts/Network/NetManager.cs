using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetManager : NetworkManager
{
    public void Start()
    {
        GameContentManager.InitContent();
        GameContentManager.LoadPrefabs();
        CameraManager.Init();
        CanvasManager.Init();

        CameraManager.SwitchByCode(0);
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

    IEnumerator InitStartContent()
    {
        SpaceManager.spaceManager = Instantiate(GameContentManager.spaceManagerPrefab);
        NetworkServer.Spawn(SpaceManager.spaceManager.gameObject);

        
        yield return 1;
    }
}
