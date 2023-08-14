using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonesBuilder : MonoBehaviour
{
    public static void Build(SpaceManager spm, StarSystem system)
    {
        for (int i = 0; i < spm.sectors.Count; i++)
        {
            LoadZones(spm, spm.sectors[i]);
        }
    }
    public static void LoadZones(SpaceManager spm, Sector sector)
    {
        if (sector == null)
        {
            return;
        }
        string sectorTemplateName = sector.templateName;
        Template currentSectorTemplate = TemplateManager.FindTemplate(sectorTemplateName, "sector");
        if (currentSectorTemplate == null)
        {
            Debug.LogError("Sector template " + sectorTemplateName + " is not found");
            return;
        }
        if (spm is MapSpaceManager)
        {
            for (int i = 0; i < spm.zones.Count; i++)
            {
                spm.zones[i].Destroy();
            }
            spm.zones = new List<Zone>();
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
                Zone zone = new Zone(spm, zoneTemplateName);
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
                List<Zone> fsps = spm.zones.FindAll(f => f.galaxyId == sector.galaxyId && f.systemId == sector.systemId && f.sectorId == sector.id);
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
                            spm.zones.Remove(zone);
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
                if (i == 0)
                {
                    zone.id = 0;
                }
                else
                {
                }
                if (spm is MapSpaceManager)
                {
                    zone.Init();
                }
                spm.zones.Add(zone);
            }
        }
    }
}
