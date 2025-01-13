using System.Collections.Generic;
using Unity.VisualScripting;
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
    public static int chunkSize = 5;
    public static bool in_process = false;
    static ChunkManager chunkManager;
    public static Template galaxyTemplate;
    public static List<TemplateNode> systemNodes = new List<TemplateNode>();
    public static List<SpaceSystem> stars = new List<SpaceSystem>();
    public static void Init()
    {
        OnFixAction += OnFix;
        chunkManager = ChunkManager.Create(chunkSize);
        chunkManager.name = "GalaxyChunkManager";
        DontDestroyOnLoad(chunkManager.gameObject);
    }
    public static SpaceSystem CreateStar(Vector3 position)
    {
        SpaceSystem star = stars.Find(x => x.hidden == true);
        bool newStar = false;

        if (star == null)
        {
            newStar = true;
            star = new SpaceSystem();
            star.Init();
        }
        List<TemplateNode> list = new List<TemplateNode>(GalaxyChunkController.systemNodes);
        float galMaxRange = XMLF.FloatVal(GalaxyChunkController.galaxyTemplate.GetValue("galaxy", "maxRange"));
        TemplateNode node = TemplateNode.GetByWeightsList(list);
        float minRange = XMLF.FloatVal(node.GetValue("minRange"));
        float maxRange = XMLF.FloatVal(node.GetValue("maxRange"));
        Vector3 galaxyStarPosition = position;
        float dst = Vector3.Distance(Vector3.zero, galaxyStarPosition);
        while ((dst < minRange || dst > maxRange) && list.Count > 1)
        {
            list.Remove(node);

            node = TemplateNode.GetByWeightsList(list);
            minRange = XMLF.FloatVal(node.GetValue("minRange"));
            maxRange = XMLF.FloatVal(node.GetValue("maxRange"));
            // Ymin = int.Parse(node.GetValue("Ymin"));
            // Ymax = int.Parse(node.GetValue("Ymax"));
            // yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
        }
        string systemTemplateName = node.GetValue("template");
        Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
        List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
        List<TemplateNode> colorBgNodes = systemTemplate.GetNodeList("bg_color");
        List<TemplateNode> skyboxes = systemTemplate.GetNodeList("skybox");
        star.SetPosition(galaxyStarPosition);
        if (colorNodes.Count > 0)
        {
            TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
            Color32 col = colorNode.GetColor();
            star.SetColor(col);
        }
        if (colorBgNodes.Count > 0)
        {
            TemplateNode colorBgNode = TemplateNode.GetByWeightsList(colorBgNodes);
            Color32 col = colorBgNode.GetColor();
            star.SetBgColor(col);
        }
        if (skyboxes.Count > 0)
        {
            TemplateNode skyboxNode = TemplateNode.GetByWeightsList(skyboxes);
            star.skyboxName = skyboxNode.GetValue("name");
        }
        star.galaxyId = LocalClient.Galaxy.id;
        star.temp = true;
        star.hidden = false;
        star.Show();
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
        Space.InvokeMinimapRender();
    }
}
