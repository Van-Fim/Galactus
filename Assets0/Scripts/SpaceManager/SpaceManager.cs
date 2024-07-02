using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    public static SpaceManager singleton;
    public static string universeTemplateName = "default";
    public static GameObject spaceContainer;
    public static GameObject solarContainer;
    public List<Galaxy> galaxies = new List<Galaxy>();
    public List<SpaceSystem> spaceSystems = new List<SpaceSystem>();
    public List<Gate> gates = new List<Gate>();
    public Galaxy currentGalaxy;
    public int emptySize = 3;
    public void LateUpdate()
    {
        SpaceUiObj.InvokeRender();
    }
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<SpaceManager>();
        spaceContainer = GamePrefabsManager.singleton.LoadPrefab<Transform>("SpaceContainer").gameObject;
        spaceContainer = Instantiate(spaceContainer);
        spaceContainer.transform.localPosition = Vector3.zero;
        spaceContainer.transform.localRotation = Quaternion.identity;
    }

    public void GenerateUniverse()
    {
        System.Random rndm = new System.Random(GameManager.seed.GetHashCode());
        UnityEngine.Random.InitState(GameManager.seed.GetHashCode());
        Template template = TemplateManager.FindTemplate(universeTemplateName, "universe");

        List<TemplateNode> nodes = template.GetNodeList("galaxy");
        TemplateNode nd = template.GetNode("galaxies");
        int maxRangeMin = int.Parse(nd.GetValue("maxRangeMin"));
        int maxRangeMax = int.Parse(nd.GetValue("maxRangeMax"));
        float range = UnityEngine.Random.Range(maxRangeMin, maxRangeMax);
        for (int j = 0; j < nodes.Count; j++)
        {
            TemplateNode node = nodes[j];

            int maxCount = int.Parse(node.GetValue("max"));
            int minCount = int.Parse(node.GetValue("min"));

            int Ymax = int.Parse(node.GetValue("Ymax"));
            int Ymin = int.Parse(node.GetValue("Ymin"));

            int size = int.Parse(node.GetValue("size"));
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            string galaxyTemplateName = node.GetValue("template");
            Template galaxyTemplate = TemplateManager.FindTemplate(galaxyTemplateName, "galaxy");
            if (galaxyTemplate == null)
            {
                Debug.LogError("Galaxy template " + galaxyTemplateName + " is not found");
                return;
            }
            List<TemplateNode> colorNodes = galaxyTemplate.GetNodeList("color");

            for (int i = 0; i < count; i++)
            {
                int height = Ymax - Ymin;
                Vector3 position = new Vector3(Random.Range(-range, range + 1), Random.Range(-height, height + 1), Random.Range(-range, range + 1));
                Galaxy fgal = galaxies.Find(x => x.GetPosition() == position);
                int tryCount = 10;
                while (fgal != null && tryCount > 0)
                {
                    position = new Vector3(Random.Range(-range, range + 1), Random.Range(-height, height + 1), Random.Range(-range, range + 1));
                    fgal = galaxies.Find(x => x.GetPosition() == position);
                    tryCount--;
                    if (tryCount == 0)
                    {
                        return;
                    }
                }
                Galaxy galaxy = new Galaxy(this, galaxyTemplateName);
                galaxy.SetPosition(position);
                if (colorNodes.Count > 0)
                {
                    TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
                    Color32 col = colorNode.GetColor();
                    galaxy.SetColor(col);
                }
            }
        }
    }
    public void GenerateSystems()
    {
        for (int i = 0; i < galaxies.Count; i++)
        {
            Galaxy galaxy = galaxies[i];

            string seed = $"{GameManager.seed}{galaxy.id.ToString()}";
            System.Random rndm = new System.Random(seed.GetHashCode());
            UnityEngine.Random.InitState(seed.GetHashCode());
            Template galaxyTemplate = TemplateManager.FindTemplate(galaxy.templateName, "galaxy");
            if (galaxyTemplate == null)
            {
                Debug.LogError("Galaxy template " + galaxy.templateName + " is not found");
                return;
            }
            int minCount = int.Parse(galaxyTemplate.GetValue("galaxy", "systems_min"));
            int maxCount = int.Parse(galaxyTemplate.GetValue("galaxy", "systems_max"));
            List<TemplateNode> nodes = galaxyTemplate.GetNodeList("system");
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            for (int cn = 0; cn < count; cn++)
            {
                TemplateNode node = TemplateNode.GetByWeightsList(nodes);
                string systemTemplateName = node.GetValue("template");
                Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
                List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
                List<TemplateNode> colorBgNodes = systemTemplate.GetNodeList("bg_color");
                List<TemplateNode> skyboxes = systemTemplate.GetNodeList("skybox");
                int Ymin = int.Parse(node.GetValue("Ymin"));
                int Ymax = int.Parse(node.GetValue("Ymax"));
                int minRange = int.Parse(node.GetValue("minRange"));
                int maxRange = int.Parse(node.GetValue("maxRange"));
                int height = Ymax - Ymin;
                int range = UnityEngine.Random.Range(minRange, maxRange);
                int sizeMin = int.Parse(systemTemplate.GetValue("system", "sizeMin"));
                int sizeMax = int.Parse(systemTemplate.GetValue("system", "sizeMax"));
                int size = UnityEngine.Random.Range(sizeMin, sizeMax + 1);
                Vector3 position = new Vector3(Random.Range(-range, range + 1), Random.Range(-height, height + 1), Random.Range(-range, range + 1));
                SpaceSystem fsys = SpaceManager.singleton.spaceSystems.Find(x => x.GetPosition() == position && x.galaxyId == galaxy.id);
                int tryCount = 10;
                while (fsys != null && tryCount > 0)
                {
                    position = new Vector3(Random.Range(-range, range + 1), Random.Range(-height, height + 1), Random.Range(-range, range + 1));
                    fsys = SpaceManager.singleton.spaceSystems.Find(x => x.GetPosition() == position && x.galaxyId == galaxy.id);
                    tryCount--;
                    if (tryCount == 0)
                    {
                        return;
                    }
                }
                SpaceSystem system = new SpaceSystem(galaxy, systemTemplateName);
                system.galaxyId = galaxy.id;
                system.SetPosition(position);
                if (colorNodes.Count > 0)
                {
                    TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
                    Color32 col = colorNode.GetColor();
                    system.SetColor(col);
                }
                if (colorBgNodes.Count > 0)
                {
                    TemplateNode colorBgNode = TemplateNode.GetByWeightsList(colorBgNodes);
                    Color32 col = colorBgNode.GetColor();
                    system.SetBgColor(col);
                }
                if (skyboxes.Count > 0)
                {
                    TemplateNode skyboxNode = TemplateNode.GetByWeightsList(skyboxes);
                    system.skyboxName = skyboxNode.GetValue("name");
                }
                system.size = size;
                system.Init();
            }
        }
    }

    public bool IsSystemConnected(SpaceSystem spaceSystem)
    {
        bool ret = false;
        List<SpaceSystem> connectedSystems = new List<SpaceSystem>();
        for (int i = 0; i < SpaceManager.singleton.spaceSystems.Count; i++)
        {
            SpaceSystem sp = SpaceManager.singleton.spaceSystems[i];
            for (int j = 0; j < sp.connectedSystems.Count; j++)
            {
                SpaceSystem csp = sp.connectedSystems[j];
                if (csp == spaceSystem)
                {
                    return true;
                }
                if (!connectedSystems.Contains(csp))
                {
                    connectedSystems.Add(csp);
                }
            }
        }
        return ret;
    }

    public static void LoadSystem(SpaceSystem spaceSystem)
    {
        Material mat = Resources.Load<Material>($"Materials/Skybox/{spaceSystem.skyboxName}");
        RenderSettings.skybox = mat;
        Color32 color = spaceSystem.GetBgColor();
        float cdiv = 1f;
        color = new Color32((byte)(color.r / cdiv), (byte)(color.g / cdiv), (byte)(color.b / cdiv), color.a);
        RenderSettings.skybox.SetColor("_Tint", color);
    }
}
