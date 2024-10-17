using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : SolarObject
{
    public override void Init()
    {
        OnRenderAction += OnRender;
    }
    public Sector(SolarObject sObj, string templateName, int minRange = 0, int maxRange = 0, bool firstSector = false)
    {
        this.parentSolarObject = sObj;
        Planet pl = null;
        Sun sn = null;
        if (sObj is Planet)
        {
            pl = (Planet)sObj;
        }
        else if (sObj is Sun)
        {
            sn = (Sun)sObj;
        }
        galaxyId = sObj.galaxyId;
        systemId = sObj.systemId;
        SpaceSystem system = SpaceManager.spaceSystems.Find(x => x.galaxyId == galaxyId && x.id == systemId);
        int solIndex = 0;
        if (pl != null)
        {
            solIndex = SpaceManager.planets.IndexOf(pl);
            this.parentSolarObject = SpaceManager.planets[solIndex];
        }
        else if (sn != null)
        {
            solIndex = SpaceManager.suns.IndexOf(sn);
            this.parentSolarObject = SpaceManager.suns[solIndex];
        }
        this.deptch = (byte)(parentSolarObject.deptch + 1);
        Template template = TemplateManager.FindTemplate(templateName, "sector");
        model = template.GetValue("model", "patch");
        int range = maxRange - minRange;
        float curDistance = 0;
        float sumDistance = 0;
        float dist2 = 1000;
        int repeatCount = 10;
        Vector3 sectorPosition = Vector3.zero;
        Vector3 plpos = Vector3.zero;
        bool found = true;

        List<TemplateNode> orbitColorNodes = template.GetNodeList("orbit_color");
        Color32 color = new Color32(255, 255, 255, 255);
        if (orbitColorNodes.Count > 0)
        {
            TemplateNode colorNode = TemplateNode.GetByWeightsList(orbitColorNodes);
            byte r = byte.Parse(colorNode.GetValue("r"));
            byte g = byte.Parse(colorNode.GetValue("g"));
            byte b = byte.Parse(colorNode.GetValue("b"));
            byte a = byte.Parse(colorNode.GetValue("a"));
            this.SetOrbitColor(new Color32(r, g, b, a));
        }
        List<TemplateNode> colorNodes = template.GetNodeList("color");
        color = new Color32(255, 255, 255, 255);
        if (colorNodes.Count > 0)
        {
            TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
            byte r = byte.Parse(colorNode.GetValue("r"));
            byte g = byte.Parse(colorNode.GetValue("g"));
            byte b = byte.Parse(colorNode.GetValue("b"));
            byte a = byte.Parse(colorNode.GetValue("a"));
            this.SetColor(new Color32(r, g, b, a));

            if (orbitColorNodes.Count == 0)
            {
                this.SetOrbitColor(new Color32(r, g, b, a));
            }

        }
        List<SolarObject> allObjects = new List<SolarObject>();
        List<Sun> suns = SpaceManager.suns.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == systemId);
        List<Planet> planets = SpaceManager.planets.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == systemId);
        List<Sector> sectors = SpaceManager.sectors.FindAll(f => f.galaxyId == system.galaxyId && f.systemId == systemId);
        allObjects.AddRange(suns);
        allObjects.AddRange(planets);
        allObjects.AddRange(sectors);
        //allObjects.AddRange(system.asteroidFields);
        if (!firstSector)
        {
            while (repeatCount > 0 && found)
            {
                Vector2 vPosition = UnityEngine.Random.insideUnitCircle * (range);
                Vector2 fPosition = (vPosition.normalized * minRange);
                Vector2 pos = fPosition + vPosition;
                plpos = new Vector3(pos.x, 0, pos.y);
                plpos = PositionFixer.RecalcPos(plpos * SolarObject.scaleFactor, PositionFixer.sectorStepSize) / SolarObject.scaleFactor;
                sectorPosition = (plpos + sObj.GetPosition());
                int allCount = allObjects.Count;
                for (int i = 0; i < allCount; i++)
                {
                    SolarObject pl1 = allObjects[i];

                    Vector3 plPos = pl1.GetPosition();
                    if (pl1.parentSolarObject != null)
                    {
                        plPos += pl1.parentSolarObject.GetPosition();
                    }

                    float dist1 = Vector3.Distance(sObj.GetPosition(), sectorPosition);
                    dist2 = 0;
                    if (pl1 != sObj)
                    {
                        dist2 = Vector2.Distance(sObj.GetPosition(), plPos);
                    }

                    curDistance = Mathf.Abs(dist1 - dist2);
                    float planetDistance = (pl1.scale + 10);
                    sumDistance = (planetDistance) * 1.5f;
                    found = false;

                    if (curDistance < sumDistance || dist1 < planetDistance)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    break;
                }
                repeatCount--;
            }

            if (found)
            {
                id = -1;
                systemId = -1;
                return;
            }
        }
        else
        {
            plpos = new Vector3(0, 0, 0);
            plpos = PositionFixer.RecalcPos(plpos * SolarObject.scaleFactor, PositionFixer.sectorStepSize) / SolarObject.scaleFactor;
        }

        SetPosition(plpos);
        galaxyId = system.galaxyId;
        systemId = system.id;
        GetId();
        SpaceManager.sectors.Add(this);
    }
    public int GetId()
    {
        int id = 0;
        while (SpaceManager.sectors.Find(f => f.id == id && f.galaxyId == galaxyId && f.systemId == systemId) != null)
        {
            id++;
        }

        this.id = id;
        return this.id;
    }
    public override void RenderAct()
    {
        if ((LocalClient.galaxyId == galaxyId && LocalClient.systemId == systemId))
        {
            if (main == null)
            {
                SpaceSystem sys = SpaceManager.spaceSystems.Find(x => x.galaxyId == galaxyId && x.id == systemId);
                solarController = new GameObject().AddComponent<SolarController>();
                if (parentSolarObject.main == null)
                {
                    parentSolarObject.OnRender();
                }
                solarController.transform.SetParent(SpaceManager.solarContainer.transform);
                solarController.transform.localPosition = GetPosition();
                solarController.transform.eulerAngles = GetRotation();
                solarController.solarObject = this;
                GameObject sunGameobject = Resources.Load<GameObject>($"{model}/MAIN");
                main = GameObject.Instantiate(sunGameobject, solarController.transform);
                float fscale = 1000000/PositionFixer.sectorStepSize;
                solarController.gameObject.layer = 7;
                GameObject hull = main.transform.Find("HULL").gameObject;

                main.transform.localScale = new Vector3(fscale, fscale, fscale);

                Color32 col = sys.GetColor();
                DrawCircle();
                //hull.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", col);
                //hull.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", col);
                /*
                sunLight = new GameObject().AddComponent<Light>();
                sunLight.transform.SetParent(SpaceManager.spaceContainer.transform);
                sunLight.transform.localPosition = GetPosition();
                sunLight.range = 1000000000;
                sunLight.color = col;
                */
                solarController.gameObject.name = "Sector" + id.ToString();
            }
        }
        else
        {
            Destroy();
        }
    }
    public void MinimapDestroy()
    {
        base.Destroy();
    }
    public override void Destroy()
    {
        galaxyId = -1;
        systemId = -1;
        id = -1;
        OnRenderAction -= OnRender;
        base.Destroy();
    }
}