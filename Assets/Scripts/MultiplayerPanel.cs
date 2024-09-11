using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPanel : MonoBehaviour
{
    public static MultiplayerPanel singleton;
    public static void Init()
    {
        Transform tr = GamePrefabsManager.LoadPrefab<Transform>("MultiplayerPanelPrefab");
        singleton = Instantiate(tr, CanvasController.singleton.transform).GetComponent<MultiplayerPanel>();
    }
    public void Server(){
        NetManager.singleton.StartServer();
        MultiplayerPanel.singleton.gameObject.SetActive(false);
    }
    public void Host(){
        NetManager.singleton.StartHost();
        MultiplayerPanel.singleton.gameObject.SetActive(false);
    }
    public void Client(){
        NetManager.singleton.StartClient();
        MultiplayerPanel.singleton.gameObject.SetActive(false);
    }
}
