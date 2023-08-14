using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorsBuilder : MonoBehaviour
{
    public static void Build(SpaceManager spm, StarSystem system)
    {
        LoadSectors(spm, system);
    }
    public static void LoadSectors(SpaceManager spm, StarSystem system)
    {
        if (spm is MapSpaceManager)
        {
            for (int i = 0; i < spm.sectors.Count; i++)
            {
                spm.sectors[i].Destroy();
            }
            spm.sectors = new List<Sector>();
        }

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
        List<Sun> suns = spm.suns.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == system.id);
        List<Planet> planets = spm.planets.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == system.id);
        List<SolarObject> allObjs = new List<SolarObject>();
        allObjs.AddRange(suns);
        allObjs.AddRange(planets);
        Sun sn = null;
        if (suns.Count > 0)
        {
            sn = suns[0];
        }
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
                Sector sector = new Sector(spm, sectorTemplateName);
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
                int dist = 0;
                if (i == 0)
                {
                    xPos = yPos = zPos = 0;

                    if (sn != null)
                    {
                        zPos = (-sn.scale) * 3;
                    }
                }
                Vector3 indexes = new Vector3(xPos, yPos, zPos);
                List<Sector> fsps = spm.sectors.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == system.id);
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
                        for (int s = 0; s < suns.Count; s++)
                        {
                            if (ind1 == indexes || Vector3.Distance(new Vector3(xPos, yPos, zPos), suns[s].GetPosition()) < dist)
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
                            break;
                        }
                    }
                    if (br)
                    {
                        counter--;
                        if (counter == 0)
                        {
                            spm.sectors.Remove(sector);
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
                sector.startPos = (indexes * size);
                sector.size = size;
                sector.galaxyId = system.galaxyId;
                sector.systemId = system.id;
                if (spm is MapSpaceManager)
                {
                    sector.Init();
                }
                spm.sectors.Add(sector);
            }
        }
    }
}
