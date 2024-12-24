using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpaceManager : MonoBehaviour
{
    public static SpaceManager singleton;

    public static List<Galaxy> galaxies = new List<Galaxy>();
    public static List<SpaceSystem> spaceSystems = new List<SpaceSystem>();
    public static List<Region> regions = new List<Region>();
    public static List<Sector> sectors = new List<Sector>();
    public static List<Gate> gates = new List<Gate>();
    public static List<Planet> planets = new List<Planet>();
    public static List<Sun> suns = new List<Sun>();
    public static GameObject spaceContainer;
    public static GameObject solarContainer;
    public static GameObject galaxyContainer;
    public static Material mat;
    public static int currentMapGalaxyId;
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<SpaceManager>();
        spaceContainer = new GameObject();
        spaceContainer.name = "SpaceContainer";
        solarContainer = new GameObject();
        solarContainer.name = "SolarContainer";
        galaxyContainer = new GameObject();
        galaxyContainer.name = "GalaxyContainer";
        GameObject.DontDestroyOnLoad(spaceContainer);
        GameObject.DontDestroyOnLoad(solarContainer);
        GameObject.DontDestroyOnLoad(galaxyContainer);
        solarContainer.transform.rotation = Quaternion.identity;
        spaceContainer.transform.rotation = Quaternion.identity;
        solarContainer.transform.position = Vector3.zero;
        spaceContainer.transform.position = Vector3.zero;
    }
    public void LateUpdate()
    {
        SpaceUiObj.InvokeRender();
    }
    public static void SetRandomSkybox()
    {
        string[] skyboxes = new string[] { "Skybox01", "Skybox02", "Skybox03", "Skybox04" };
        int rnd = UnityEngine.Random.Range(0, skyboxes.Length - 1);
        mat = Resources.Load<Material>($"Materials/Skybox/{skyboxes[rnd]}");
        RenderSettings.skybox = mat;
    }
    public static void LoadSystem(SpaceSystem spaceSystem)
    {
        if (spaceSystem.skyboxName == null)
        {
            return;
        }
        Material mat = Resources.Load<Material>($"Materials/Skybox/{spaceSystem.skyboxName}");
        RenderSettings.skybox = mat;
        Color32 color = spaceSystem.GetBgColor();
        float cdiv = 1f;
        color = new Color32((byte)(color.r / cdiv), (byte)(color.g / cdiv), (byte)(color.b / cdiv), color.a);
        RenderSettings.skybox.SetColor("_Tint", color);
    }

    public static void BuildGalaxies()
    {
        int seed = GameManager.GetSeed();
        System.Random rndm = new System.Random(seed);
        UnityEngine.Random.InitState(seed);
        Template template = TemplateManager.FindTemplate(LocalClient.universeTemplateName, "universe");

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
                Vector3 position = new Vector3(UnityEngine.Random.Range(-range, range + 1), UnityEngine.Random.Range(-height, height + 1), UnityEngine.Random.Range(-range, range + 1));
                Galaxy fgal = galaxies.Find(x => x.GetPosition() == position);
                int tryCount = 10;
                while (fgal != null && tryCount > 0)
                {
                    position = new Vector3(UnityEngine.Random.Range(-range, range + 1), UnityEngine.Random.Range(-height, height + 1), UnityEngine.Random.Range(-range, range + 1));
                    fgal = galaxies.Find(x => x.GetPosition() == position);
                    tryCount--;
                    if (tryCount == 0)
                    {
                        return;
                    }
                }
                Galaxy galaxy = new Galaxy(galaxyTemplateName);
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
    public static void BuildSystems()
    {
        for (int i = 0; i < galaxies.Count; i++)
        {
            Galaxy galaxy = galaxies[i];
            int seed = GameManager.GetSeed(galaxy.id);
            System.Random rndm = new System.Random(seed);
            UnityEngine.Random.InitState(seed);
            Template galaxyTemplate = TemplateManager.FindTemplate(galaxy.templateName, "galaxy");
            if (galaxyTemplate == null)
            {
                Debug.LogError("Galaxy template " + galaxy.templateName + " is not found");
                return;
            }
            int numOfArms = int.Parse(galaxyTemplate.GetValue("galaxy", "numOfArms"));
            float spin = XMLF.FloatVal(galaxyTemplate.GetValue("galaxy", "spin"));
            float armSpread = XMLF.FloatVal(galaxyTemplate.GetValue("galaxy", "armSpread"));
            float starsAtCenterRatio = XMLF.FloatVal(galaxyTemplate.GetValue("galaxy", "starsAtCenterRatio"));
            int minCount = int.Parse(galaxyTemplate.GetValue("galaxy", "systems_min"));
            int maxCount = int.Parse(galaxyTemplate.GetValue("galaxy", "systems_max"));

            List<TemplateNode> nodes = galaxyTemplate.GetNodeList("system");
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            float galMaxRange = XMLF.FloatVal(galaxyTemplate.GetValue("galaxy", "maxRange"));
            // for (int g = 0; g < nodes.Count; g++)
            // {
            //     float vmx = XMLF.FloatVal(nodes[g].GetValue("maxRange"));
            //     if (vmx > galMaxRange)
            //     {
            //         galMaxRange = vmx;
            //     }
            // }
            for (int k = 0; k < numOfArms; k++)
            {
                GenerateArm(seed, galaxy, nodes, count / numOfArms, (float)k / (float)numOfArms, spin, armSpread, starsAtCenterRatio, galMaxRange);
            }
            galaxy.spaceSystems = SpaceManager.spaceSystems.FindAll(x => x.galaxyId == galaxy.id);
        }
    }
    public static void BuildRegions()
    {
        for (int i = 0; i < galaxies.Count; i++)
        {
            Galaxy galaxy = galaxies[i];
            int seed = GameManager.GetSeed(galaxy.id);
            System.Random rndm = new System.Random(seed);
            UnityEngine.Random.InitState(seed);
            Template galaxyTemplate = TemplateManager.FindTemplate(galaxy.templateName, "galaxy");
            if (galaxyTemplate == null)
            {
                Debug.LogError("Galaxy template " + galaxy.templateName + " is not found");
                return;
            }
            List<TemplateNode> nodes = galaxyTemplate.GetNodeList("region");
            for (int k = 0; k < nodes.Count; k++)
            {
                TemplateNode nd = nodes[k];
                float galMaxRange = XMLF.FloatVal(galaxyTemplate.GetValue("galaxy", "maxRange"));
                int countMin = int.Parse(nd.GetValue("count_min"));
                int countMax = int.Parse(nd.GetValue("count_max"));
                int scaleMin = int.Parse(nd.GetValue("scale_min"));
                int scaleMax = int.Parse(nd.GetValue("scale_max"));
                int count = UnityEngine.Random.Range(countMin, countMax + 1);
                string RegionTemplateName = nd.GetValue("template");
                for (int j = 0; j < count; j++)
                {
                    int Ymin = int.Parse(nd.GetValue("Ymin"));
                    int Ymax = int.Parse(nd.GetValue("Ymax"));
                    int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                    int scale = UnityEngine.Random.Range(scaleMin, scaleMax + 1);
                    float minRange = XMLF.FloatVal(nd.GetValue("minRange"));
                    float maxRange = XMLF.FloatVal(nd.GetValue("maxRange"));
                    float range = UnityEngine.Random.Range(minRange, maxRange + 1);
                    Vector2 position2D = UnityEngine.Random.insideUnitCircle * (maxRange);
                    Vector3 pos = new Vector3(position2D.x, yPos, position2D.y);
                    float dst = Vector3.Distance(Vector3.zero, pos);
                    int trys = 25;
                    while ((dst < minRange || dst > maxRange) && trys > 0)
                    {
                        trys--;
                        yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                        position2D = UnityEngine.Random.insideUnitCircle * (maxRange);
                        pos = new Vector3(position2D.x, yPos, position2D.y);
                        dst = Vector3.Distance(Vector3.zero, pos);
                    }
                    if (trys <= 0)
                    {
                        continue;
                    }
                    Region region = new Region(galaxy, RegionTemplateName);
                    region.scale = scale;
                    region.SetPosition(pos);
                    region.Init();
                }
            }
        }
    }
    public static void BuildSystemsContent()
    {
        for (int i = 0; i < galaxies.Count; i++)
        {
            Galaxy galaxy = galaxies[i];
            for (int j = 0; j < spaceSystems.Count; j++)
            {
                SpaceSystem spaceSystem = spaceSystems[j];
                if (spaceSystem.galaxyId != galaxy.id)
                {
                    continue;
                }
                List<SpaceObjectData> dataList = SpaceObjectManager.ReadSpaceContent(spaceSystem);
                SpaceObjectManager.BuildObjectsByData(dataList);
            }
        }
    }

    //--------------------------------------------
    public static double Pow3Constrained(double x)
    {
        double value = Math.Pow(x - 0.5, 3) * 4 + 0.5d;
        return Math.Max(Math.Min(1, value), 0);
    }
    //--------------------------------------------
    public static void GenerateArm(int galaxySeed, Galaxy galaxy, List<TemplateNode> nodes, int numOfStars, float rotation, float spin, double armSpread, double starsAtCenterRatio, float galMaxRange)
    {
        System.Random r = new System.Random(galaxySeed);
        for (int i = 0; i < numOfStars; i++)
        {
            double part = (double)i / (double)numOfStars;
            part = Math.Pow(part, starsAtCenterRatio);

            float distanceFromCenter = (float)part;
            double position = (part * spin + rotation) * Math.PI * 2;

            double xFluctuation = (Pow3Constrained(r.NextDouble()) - Pow3Constrained(r.NextDouble())) * armSpread;
            double yFluctuation = (Pow3Constrained(r.NextDouble()) - Pow3Constrained(r.NextDouble())) * armSpread;

            float resultX = (float)Math.Cos(position) * distanceFromCenter / 2 + 0.5f + (float)xFluctuation;
            float resultY = (float)Math.Sin(position) * distanceFromCenter / 2 + 0.5f + (float)yFluctuation;
            List<TemplateNode> starList = new List<TemplateNode>(nodes);
            TemplateNode node = TemplateNode.GetByWeightsList(starList);
            //TemplateNode node = nodes[0];

            int Ymin = int.Parse(node.GetValue("Ymin"));
            int Ymax = int.Parse(node.GetValue("Ymax"));
            int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
            Vector3 galaxyStarPosition = new Vector3(resultX, 0, resultY) * galMaxRange;

            float minRange = XMLF.FloatVal(node.GetValue("minRange"));
            float maxRange = XMLF.FloatVal(node.GetValue("maxRange"));

            Vector3 mpos = new Vector3((galMaxRange / 2), 0, (galMaxRange / 2));
            float dst = Vector3.Distance(mpos, galaxyStarPosition);
            while ((dst < minRange || dst > maxRange) && starList.Count > 1)
            {
                starList.Remove(node);

                node = TemplateNode.GetByWeightsList(starList);
                minRange = XMLF.FloatVal(node.GetValue("minRange"));
                maxRange = XMLF.FloatVal(node.GetValue("maxRange"));
                Ymin = int.Parse(node.GetValue("Ymin"));
                Ymax = int.Parse(node.GetValue("Ymax"));
                yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
            }
            string systemTemplateName = node.GetValue("template");
            galaxyStarPosition = new Vector3(galaxyStarPosition.x, yPos, galaxyStarPosition.z) - mpos;
            Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
            List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
            List<TemplateNode> colorBgNodes = systemTemplate.GetNodeList("bg_color");
            List<TemplateNode> skyboxes = systemTemplate.GetNodeList("skybox");
            SpaceSystem system = new SpaceSystem(galaxy, systemTemplateName);
            system.SetPosition(galaxyStarPosition);
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

            system.Init();
        }
    }
}
