using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RandomAreaSpawner randomAreaSpawner;
    public static GameManager singleton;
    public string seed = "mygameSeed";
    
    void Awake()
    {
        singleton = this;
    }
    void Start(){
        Init();
    }
    void Init()
    {
        GameContentManager.InitContent();
        GameContentManager.LoadPrefabs();

        SpaceManager.Init();

        SPObjectManager.Init();

        CameraManager.Init();
        CameraManager.SwitchByCode(0);

        CanvasManager.Init();
        // ClientPanelManager.Init();
        // ClientPanelManager.Show<HudClientPanel>();
        // Client.Init();

        // SpaceManager.singleton.Load();
        // GameStartData gameStartData = GameStartData.Init("Start01");
        // Client.localClient.galaxyId = gameStartData.galaxyId;
        // Client.localClient.systemId = gameStartData.starSystemId;
        // Client.localClient.sectorId = gameStartData.sectorId;
        // Client.localClient.zoneId = gameStartData.zoneId;
        // Client.localClient.ReadSpace();
        // gameStartData.LoadContent();

        // SpaceManager.singleton.WarpClient(gameStartData.galaxyId, gameStartData.starSystemId, gameStartData.sectorId, gameStartData.zoneId);
    }
}
