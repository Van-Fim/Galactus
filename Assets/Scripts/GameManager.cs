using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace GameContent.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager singleton;
        private string seed = "mygameSeed";
        public bool isGameInitialized;
        public static bool isInMainMenu = true;
        void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
            }
        }
        void Start()
        {
            if (!singleton.isGameInitialized)
            {
                Init();
            }
        }
        void Init()
        {
            GamePrefabsManager.Init();
            GamePrefabsManager.singleton.InitContent();

            CanvasManager.Init();
            CameraManager.Init();
            ClientInputManager.Init();
            ClientPanelManager.Init();
            ClientPanelManager.Show<MainMenuPanel>();

            DebugConsole.Init();

            isGameInitialized = true;
            DontDestroyOnLoad(this);
        }
        public static int GetSeed()
        {
            return singleton.seed.GetHashCode();
        }
    }
}