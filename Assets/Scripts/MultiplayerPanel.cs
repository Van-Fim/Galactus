using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using Mirror;
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
using UnityEngine;

public class MultiplayerPanel : MonoBehaviour
{
    public static MultiplayerPanel singleton;
<<<<<<< HEAD
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
=======
    public void Awake(){
        singleton = this;
    }
    public void StartServer()
    {
        NetworkManager.singleton.StartServer();
        singleton.gameObject.SetActive(false);
    }
    public void StartHost()
    {
        NetworkManager.singleton.StartHost();
        singleton.gameObject.SetActive(false);
    }
    public void StartClient()
    {
        NetworkManager.singleton.StartClient();
        singleton.gameObject.SetActive(false);
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    }
}
