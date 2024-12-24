using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class GalaxyChunkController:MonoBehaviour
{
    public static GalaxyChunkController singleton;
    public static Vector3 mapCameraCurrentIndexes = Vector3.zero;
    public static Vector3 mapCameraIndexes = Vector3.zero;
    public static UnityAction OnFixAction;
    public static Transform testCube;
    public static int chunkSize = 50;
    public static bool in_process = false;
    static ChunkManager chunkManager;
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<GalaxyChunkController>();
        OnFixAction += OnFix;

        chunkManager = new GameObject().AddComponent<ChunkManager>();
        chunkManager.chunkSize = chunkSize;
        chunkManager.name = "GalaxyChunkManager";
        DontDestroyOnLoad(chunkManager.gameObject);
    }
    public static void OnFix()
    {
        mapCameraIndexes = mapCameraCurrentIndexes;
        if (in_process)
        {
            chunkManager.UpdateChunks(mapCameraIndexes);
        }
    }
}
