using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class SpaceManager : MonoBehaviour
{
    public static List<Galaxy> galaxies = new List<Galaxy>();
    public static List<StarSystem> starSystems = new List<StarSystem>();
    public static List<Sector> sectors = new List<Sector>();
    public static List<Zone> zones = new List<Zone>();

    public static List<string> names = new List<string>();
    public static SpaceManager singleton;

    public static GameObject spaceContainer;

    public static void Init()
    {
        singleton = new GameObject().AddComponent<SpaceManager>();
        singleton.name = "SpaceManager";
    }

    public void Load()
    {
        spaceContainer = new GameObject();
        spaceContainer.name = "SpaceContainer";
        spaceContainer.transform.position = Vector3.zero;
        spaceContainer.transform.rotation = Quaternion.identity;

        UnityEngine.Random.InitState(GameManager.singleton.seed.GetHashCode());
        LoadGalaxies("default");

        for (int i = 0; i < galaxies.Count; i++)
        {
            Galaxy galaxy = galaxies[i];
            LoadSystems(galaxy);
        }

        for (int i = 0; i < starSystems.Count; i++)
        {
            StarSystem system = starSystems[i];
            LoadSectors(system);
        }

        for (int i = 0; i < sectors.Count; i++)
        {
            Sector sector = sectors[i];
            LoadZones(sector);
        }
    }

    public static Galaxy GetGalaxyByID(int id)
    {
        Galaxy galaxy = SpaceManager.galaxies.Find(f => f.id == id);
        return galaxy;
    }

    public static StarSystem GetSystemByID(int galaxyId, int id)
    {
        StarSystem system = SpaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == id);
        return system;
    }

    public static Sector GetSectorByID(int galaxyId, int systemId, int id)
    {
        Sector sector = SpaceManager.sectors.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.id == id);
        return sector;
    }

    public static Zone GetZoneByID(int galaxyId, int systemId, int sectorId, int id)
    {
        Zone zone = SpaceManager.zones.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.sectorId == sectorId && f.id == id);
        return zone;
    }

    public List<Galaxy> GetGalaxiesList()
    {
        List<Galaxy> ret = new List<Galaxy>();
        for (int i = 0; i < galaxies.Count; i++)
        {
            ret.Add(galaxies[i]);
        }
        return ret;
    }

    public static string RandomName()
    {
        string ret = "";
        int countMin = 4;
        int countMax = 10;
        bool rep = false;
        int tryCnt = 5;
        do
        {
            int maxCountChar = 2;
            string list1 = "aeioyu";
            string list2 = "qwrtpsdfghjklzxcvbnm";
            int count = UnityEngine.Random.Range(countMax, countMax + 1) / 2;
            string s1 = "";
            string s2 = "";


            for (int i = 0; i < count; i++)
            {
                int rndId = UnityEngine.Random.Range(0, 2);
                if (rndId == 0)
                {
                    s1 = list1;
                    s2 = list2;
                }
                else
                {
                    s1 = list2;
                    s2 = list1;
                }
                int j = UnityEngine.Random.Range(0, s1.Length);
                char v1 = s1[j];
                if (i == 0)
                {
                    v1 = Char.ToUpper(v1);
                }
                ret += v1;
                j = UnityEngine.Random.Range(0, s2.Length);
                v1 = s2[j];
                ret += v1;
            }
            if (names.Contains(ret))
            {
                rep = true;
                tryCnt--;
                continue;
            }
            else
            {
                names.Add(ret);
            }

        } while (rep && tryCnt > 0);

        return ret;
    }

    public void LoadGalaxies(string universeTemplateName)
    {
        Template currentUniversetemplate = TemplateManager.FindTemplate(universeTemplateName, "universe");
        if (currentUniversetemplate == null)
        {
            Debug.LogError("Universe template " + universeTemplateName + " is not found");
            return;
        }
        List<TemplateNode> nodes = currentUniversetemplate.GetNodeList("galaxy");
        TemplateNode nd = currentUniversetemplate.GetNode("galaxies");
        int maxRangeMin = int.Parse(nd.GetValue("maxRangeMin"));
        int maxRangeMax = int.Parse(nd.GetValue("maxRangeMax"));
        int range = UnityEngine.Random.Range(maxRangeMin, maxRangeMax + 1);

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
                Galaxy galaxy = new Galaxy(galaxyTemplateName);
                if (colorNodes.Count > 0)
                {
                    TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
                    byte r = byte.Parse(colorNode.GetValue("r"));
                    byte g = byte.Parse(colorNode.GetValue("g"));
                    byte b = byte.Parse(colorNode.GetValue("b"));
                    byte a = byte.Parse(colorNode.GetValue("a"));

                    galaxy.color = new byte[] { r, g, b, a };
                }
                int counter = 10;
                Vector2 position2D = UnityEngine.Random.insideUnitCircle * range;
                int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                Vector3 position = new Vector3(position2D.x, yPos, position2D.y);
                while (counter > 0)
                {
                    bool br = false;
                    for (int i1 = 0; i1 < galaxies.Count; i1++)
                    {
                        Galaxy galaxy1 = galaxies[i1];

                        if (galaxy1 == galaxy)
                        {
                            continue;
                        }
                        Vector3 pos1 = galaxy1.GetPosition();
                        float dst = Vector3.Distance(pos1, position);
                        if (dst < size)
                        {
                            position2D = UnityEngine.Random.insideUnitCircle * range;
                            yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                            position = new Vector3(position2D.x, yPos, position2D.y);
                            br = true;
                            break;
                        }
                    }
                    if (br)
                    {
                        counter--;
                        if (counter == 0)
                        {
                            galaxies.Remove(galaxy);
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                galaxy.SetPosition(position);
                galaxy.name = RandomName();
                galaxy.GenerateId();
                galaxy.Init();
                galaxies.Add(galaxy);
            }
        }
        names.Clear();
    }

    public void LoadSectors(StarSystem system)
    {
        string systemTemplateName = system.templateName;
        Template currentSystemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
        if (currentSystemTemplate == null)
        {
            Debug.LogError("System template " + systemTemplateName + " is not found");
            return;
        }
        List<TemplateNode> nodes = currentSystemTemplate.GetNodeList("sector");
        TemplateNode nd = currentSystemTemplate.GetNode("sectors");
        int maxRangeMin = int.Parse(nd.GetValue("maxSectorRangeMin"));
        int maxRangeMax = int.Parse(nd.GetValue("maxSectorRangeMax"));
        for (int j = 0; j < nodes.Count; j++)
        {
            TemplateNode node = nodes[j];

            int maxCount = int.Parse(node.GetValue("max"));
            int minCount = int.Parse(node.GetValue("min"));

            int Ymax = int.Parse(node.GetValue("Ymax"));
            int Ymin = int.Parse(node.GetValue("Ymin"));

            int size = Sector.sectorStep;
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            string sectorTemplateName = node.GetValue("template");
            Template sectorTemplate = TemplateManager.FindTemplate(sectorTemplateName, "sector");
            if (sectorTemplate == null)
            {
                Debug.LogError("Sector template " + sectorTemplateName + " is not found");
                return;
            }
            List<TemplateNode> colorNodes = sectorTemplate.GetNodeList("color");

            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    sectorTemplateName = "Sector00";
                }
                Sector sector = new Sector(sectorTemplateName);
                if (colorNodes.Count > 0)
                {
                    TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
                    sector.SetColor(colorNode.GetColor());
                }
                int counter = 10;
                Vector2 rndPos = UnityEngine.Random.insideUnitCircle * maxRangeMax;
                int xPos = (int)rndPos.x;
                int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                int zPos = (int)rndPos.y;
                if (i == 0)
                {
                    xPos = yPos = zPos = 0;
                }
                Vector3 indexes = new Vector3(xPos, yPos, zPos);
                List<Sector> fsps = sectors.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == system.id);
                while (counter > 0)
                {
                    bool br = false;
                    for (int i1 = 0; i1 < fsps.Count; i1++)
                    {
                        Sector sector1 = fsps[i1];

                        if (sector1 == sector)
                        {
                            continue;
                        }
                        Vector3 ind1 = sector1.GetIndexes();
                        if (ind1 == indexes)
                        {
                            xPos = UnityEngine.Random.Range(maxRangeMin, maxRangeMax + 1);
                            yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                            zPos = UnityEngine.Random.Range(maxRangeMin, maxRangeMax + 1);

                            indexes = new Vector3(xPos, yPos, zPos);
                            br = true;
                            break;
                        }
                    }
                    if (br)
                    {
                        counter--;
                        if (counter == 0)
                        {
                            sectors.Remove(sector);
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                sector.SetIndexes(indexes);
                sector.SetPosition(indexes * size);
                sector.size = size;
                sector.galaxyId = system.galaxyId;
                sector.systemId = system.id;
                sector.name = RandomName();
                if (i == 0)
                {
                    sector.id = 0;
                }
                else
                {
                    sector.GenerateId();
                }
                sector.Init();
                sectors.Add(sector);
            }
        }
        names.Clear();
    }

    public void LoadZones(Sector sector)
    {
        string sectorTemplateName = sector.templateName;
        Template currentSectorTemplate = TemplateManager.FindTemplate(sectorTemplateName, "sector");
        if (currentSectorTemplate == null)
        {
            Debug.LogError("Sector template " + sectorTemplateName + " is not found");
            return;
        }
        List<TemplateNode> nodes = currentSectorTemplate.GetNodeList("zone");
        TemplateNode nd = currentSectorTemplate.GetNode("zones");
        int maxRangeMin = int.Parse(nd.GetValue("maxZoneRangeMin"));
        int maxRangeMax = int.Parse(nd.GetValue("maxZoneRangeMax"));

        for (int j = 0; j < nodes.Count; j++)
        {
            TemplateNode node = nodes[j];

            int maxCount = int.Parse(node.GetValue("max"));
            int minCount = int.Parse(node.GetValue("min"));

            int Ymax = int.Parse(node.GetValue("Ymax"));
            int Ymin = int.Parse(node.GetValue("Ymin"));

            int size = Zone.zoneStep;
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            string zoneTemplateName = node.GetValue("template");
            Template zoneTemplate = TemplateManager.FindTemplate(zoneTemplateName, "zone");
            if (zoneTemplate == null)
            {
                Debug.LogError("Zone template " + zoneTemplateName + " is not found");
                return;
            }
            List<TemplateNode> colorNodes = zoneTemplate.GetNodeList("color");

            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    zoneTemplateName = "Zone00";
                }
                Zone zone = new Zone(zoneTemplateName);
                if (colorNodes.Count > 0)
                {
                    TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
                    zone.SetColor(colorNode.GetColor());
                }
                int counter = 10;
                Vector2 rndPos = UnityEngine.Random.insideUnitCircle * maxRangeMax;
                int xPos = (int)rndPos.x;
                int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                int zPos = (int)rndPos.y;
                if (i == 0)
                {
                    xPos = yPos = zPos = 0;
                }
                Vector3 indexes = new Vector3(xPos, yPos, zPos);
                List<Zone> fsps = zones.FindAll(f => f.galaxyId == sector.galaxyId && f.systemId == sector.systemId && f.sectorId == sector.id);
                while (counter > 0)
                {
                    bool br = false;
                    for (int i1 = 0; i1 < fsps.Count; i1++)
                    {
                        Zone zone1 = fsps[i1];

                        if (zone1 == zone)
                        {
                            continue;
                        }

                        Vector3 ind1 = zone1.GetIndexes();
                        if (ind1 == indexes)
                        {
                            xPos = UnityEngine.Random.Range(maxRangeMin, maxRangeMax + 1);
                            yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                            zPos = UnityEngine.Random.Range(maxRangeMin, maxRangeMax + 1);

                            indexes = new Vector3(xPos, yPos, zPos);
                            br = true;
                            break;
                        }
                    }
                    if (br)
                    {
                        counter--;
                        if (counter == 0)
                        {
                            zones.Remove(zone);
                        }
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                zone.SetIndexes(indexes);
                zone.SetPosition(indexes * size);
                zone.size = size;
                zone.galaxyId = sector.galaxyId;
                zone.systemId = sector.systemId;
                zone.sectorId = sector.id;
                zone.name = RandomName();
                if (i == 0)
                {
                    zone.id = 0;
                }
                else
                {
                    zone.GenerateId();
                }
                zone.Init();
                zones.Add(zone);
            }
        }
        names.Clear();
    }

    public static double Pow3Constrained(double x)
    {
        double value = Math.Pow(x - 0.5, 3) * 4 + 0.5d;
        return Math.Max(Math.Min(1, value), 0);
    }
    public void LoadSystems(Galaxy galaxy)
    {
        if (galaxy == null)
        {
            Debug.LogError("Galaxy is null");
            return;
        }
        Template currentGalaxytemplate = TemplateManager.FindTemplate(galaxy.templateName, "galaxy");
        if (currentGalaxytemplate == null)
        {
            Debug.LogError("Galaxy template " + galaxy.templateName + " is not found");
            return;
        }
        int numOfArms = int.Parse(currentGalaxytemplate.GetValue("galaxy", "numOfArms"));
        float spin = XMLF.FloatVal(currentGalaxytemplate.GetValue("galaxy", "spin"));
        float armSpread = XMLF.FloatVal(currentGalaxytemplate.GetValue("galaxy", "armSpread"));
        float starsAtCenterRatio = XMLF.FloatVal(currentGalaxytemplate.GetValue("sector", "starsAtCenterRatio"));

        int minCount = int.Parse(currentGalaxytemplate.GetValue("galaxy", "systems_min"));
        int maxCount = int.Parse(currentGalaxytemplate.GetValue("galaxy", "systems_max"));

        int numOfStars = UnityEngine.Random.Range(minCount, maxCount + 1);

        string galaxySeed = (GameManager.singleton.seed + "_galaxy_" + galaxy.id.ToString());
        string galaxyType = currentGalaxytemplate.GetValue("galaxy", "type");
        UnityEngine.Random.InitState(galaxySeed.GetHashCode());
        List<TemplateNode> nodes = currentGalaxytemplate.GetNodeList("system");
        float galMaxRange = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            float vmx = XMLF.FloatVal(nodes[i].GetValue("maxRange"));
            if (vmx > galMaxRange)
            {
                galMaxRange = vmx;
            }
        }
        int count = UnityEngine.Random.Range(minCount, maxCount + 1);
        UnityEngine.Random.InitState(galaxySeed.GetHashCode());

        if (galaxyType == "galaxy")
        {
            for (int i = 0; i < numOfArms; i++)
            {
                GenerateArm(galaxySeed, galaxy, nodes, numOfStars / numOfArms, (float)i / (float)numOfArms, spin, armSpread, starsAtCenterRatio, galMaxRange);
            }
        }
        else if (galaxyType == "default")
        {
            for (int i = 0; i < count; i++)
            {
                GenerateSystem(galaxySeed, galaxy, nodes, galMaxRange);
            }
        }
    }
    public static void GenerateArm(string galaxySeed, Galaxy galaxy, List<TemplateNode> nodes, int numOfStars, float rotation, float spin, double armSpread, double starsAtCenterRatio, float galMaxRange)
    {
        System.Random r = new System.Random(galaxySeed.GetHashCode());
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
            Vector3 galaxyStarPosition = new Vector3(resultX, yPos, resultY) * galMaxRange;
            float minRange = XMLF.FloatVal(node.GetValue("minRange"));
            float maxRange = XMLF.FloatVal(node.GetValue("maxRange"));

            Vector3 mpos = new Vector3((galMaxRange / 2), 0, (galMaxRange / 2));
            float dst = Vector2.Distance(mpos, galaxyStarPosition);
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
            StarSystem starSystem = new StarSystem(systemTemplateName, galaxy);
            starSystem.SetPosition(galaxyStarPosition);
            Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
            List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
            TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
            List<TemplateNode> colorbgNodes = systemTemplate.GetNodeList("bg_color");
            TemplateNode colorbgNode = TemplateNode.GetByWeightsList(colorbgNodes);
            List<TemplateNode> skyboxNodes = systemTemplate.GetNodeList("skybox");
            TemplateNode skyboxNode = TemplateNode.GetByWeightsList(skyboxNodes);

            starSystem.name = RandomName();
            starSystem.templateName = systemTemplateName;
            starSystem.skyboxName = skyboxNode.GetValue("name");
            starSystem.GenerateId();
            // int prevSeed = UnityEngine.Random.state.GetHashCode();
            // UnityEngine.Random.InitState($"system_{starSystem.id}_{galaxySeed}".GetHashCode());
            starSystem.SetColor(colorNode.GetColor());
            if (colorbgNode != null)
            {
                starSystem.SetBgColor(colorbgNode.GetColor());
            }
            // UnityEngine.Random.InitState(prevSeed);
            starSystem.Init();
            starSystems.Add(starSystem);
        }
    }
    public static void GenerateSystem(string galaxySeed, Galaxy galaxy, List<TemplateNode> nodes, float galMaxRange)
    {
        System.Random r = new System.Random(galaxySeed.GetHashCode());

        TemplateNode node = TemplateNode.GetByWeightsList(nodes);
        int counter = 10;
        string systemTemplateName = node.GetValue("template");
        Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
        Vector2 position2D = UnityEngine.Random.insideUnitCircle * galMaxRange;
        int Ymin = int.Parse(node.GetValue("Ymin"));
        int Ymax = int.Parse(node.GetValue("Ymax"));
        int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
        Vector3 position = new Vector3(position2D.x, yPos, position2D.y);
        StarSystem system = new StarSystem(systemTemplateName, galaxy);
        system.SetPosition(position);
        List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
        TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);

        List<TemplateNode> colorbgNodes = systemTemplate.GetNodeList("bg_color");
        TemplateNode colorbgNode = TemplateNode.GetByWeightsList(colorbgNodes);

        List<TemplateNode> skyboxNodes = systemTemplate.GetNodeList("skybox");
        TemplateNode skyboxNode = TemplateNode.GetByWeightsList(skyboxNodes);

        system.name = RandomName();
        system.templateName = systemTemplateName;
        system.skyboxName = skyboxNode.GetValue("name");
        while (counter > 0)
        {
            bool br = false;
            for (int i1 = 0; i1 < starSystems.Count; i1++)
            {
                StarSystem system1 = starSystems[i1];

                if (system1 == system || system1.galaxyId != galaxy.id)
                {
                    continue;
                }
                Vector2 pos1 = system1.GetPosition();
                float dst = Vector2.Distance(pos1, position);
                if (dst < 100)
                {
                    position2D = UnityEngine.Random.insideUnitCircle * galMaxRange;
                    yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                    position = new Vector3(position2D.x, yPos, position2D.y);
                    br = true;
                    break;
                }
            }
            if (br)
            {
                counter--;
                if (counter == 0)
                {
                    starSystems.Remove(system);
                }
                continue;
            }
            else
            {
                system.SetPosition(position);
                system.GenerateId();
                // int prevSeed = UnityEngine.Random.state.GetHashCode();
                // UnityEngine.Random.InitState($"system_{system.id}_{galaxySeed}".GetHashCode());
                system.SetColor(colorNode.GetColor());
                if (colorbgNode != null)
                {
                    system.SetBgColor(colorbgNode.GetColor());
                }
                // UnityEngine.Random.InitState(prevSeed);
                system.Init();
                starSystems.Add(system);
                break;
            }
        }
    }
    public void LateUpdate()
    {
        SpaceUiObj.InvokeRender();
    }
}
