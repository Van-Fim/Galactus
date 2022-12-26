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
        SpaceManager.singleton = Instantiate(GameContentManager.spaceManagerPrefab);
        NetworkServer.Spawn(SpaceManager.singleton.gameObject);
        yield return 1;
    }
}
