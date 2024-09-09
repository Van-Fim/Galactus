using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetManager : NetworkManager
{
    public override void OnStartServer()
    {
        MenuManager.singleton.gameObject.SetActive(false);
        PositionFixer.Init();

        SpaceManager.Init();
        SpaceObjectManager.Init();
        IND_targetManager.Init();
        SpaceManager.BuildGalaxies();
        SpaceManager.BuildSystems();
        SpaceManager.BuildSystemsContent();
        LocalClient.galaxyId = 0;
        LocalClient.systemId = 0;
        SpaceManager.LoadSystem(LocalClient.SpaceSystem);
    }
}
