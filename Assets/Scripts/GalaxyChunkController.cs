using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class GalaxyChunkController : MonoBehaviour
{
    public static GalaxyChunkController singleton;
    public static Vector3 mapCameraCurrentIndexes = Vector3.zero;
    public static Vector3 mapCameraIndexes = Vector3.zero;
    public static UnityAction OnFixAction;
    public static Transform testCube;
    public static int chunkSize = 50;
    public static bool in_process = false;
    static ChunkManager chunkManager;
    public static Template galaxyTemplate;
    public static List<TemplateNode> systemNodes = new List<TemplateNode>();
    public static List<Star> stars = new List<Star>();
    public static void Init()
    {
        OnFixAction += OnFix;

        chunkManager = new GameObject().AddComponent<ChunkManager>();
        chunkManager.chunkSize = chunkSize;
        chunkManager.name = "GalaxyChunkManager";
        DontDestroyOnLoad(chunkManager.gameObject);
    }
    public static Star CreateStar(Vector3 position)
    {
        Star star = stars.Find(x => x.hidden == true);
        bool newStar = false;
        if (star == null)
        {
            newStar = true;
            star = new Star();
            star.obj = GameObject.Instantiate(GamePrefabsManager.LoadPrefab<Transform>("TestCube"));
            star.obj.gameObject.name = "TestCube";
            star.obj.gameObject.layer = 6;
            star.obj.transform.SetParent(SpaceManager.galaxyContainer.transform);
        }
        star.obj.localScale = new Vector3(chunkSize, chunkSize, chunkSize);
        star.obj.transform.localPosition = position;
        star.SetHiddenState(false);
        if (newStar)
        {
            stars.Add(star);
        }
        return star;
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
