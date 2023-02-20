using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Planet : SolarObject
{
    public static UnityAction OnTick;
    [Range(0f, 1f)]
    float orbitProgress = 0f;
    float orbitProgressAdd = 0f;

    float rotateSpeed = 0f;
    bool orbitActive = true;

    public Planet() { }
    public Planet(StarSystem system, Sun sun, string templateName, int minRange = 0, int maxRange = 0)
    {
        this.parentSolarObject = sun;
        galaxyId = system.galaxyId;
        systemId = system.id;
        template = TemplateManager.FindTemplate(templateName, "planet");
        model = template.GetValue("model", "patch");
        int scaleMin = int.Parse(template.GetValue("scale", "min"));
        int scaleMax = int.Parse(template.GetValue("scale", "max"));

        this.scale = Random.Range(scaleMin, scaleMax + 1);

        float speedMin = XMLF.FloatVal(template.GetValue("orbitPeriod", "min"));
        float speedMax = XMLF.FloatVal(template.GetValue("orbitPeriod", "max"));
        float speedRotMin = XMLF.FloatVal(template.GetValue("rotateSpeed", "min"));
        float speedRotMax = XMLF.FloatVal(template.GetValue("rotateSpeed", "max"));
        this.orbitPeriod = Random.Range(speedMin, speedMax + 1);
        this.rotateSpeed = Random.Range(speedRotMin, speedRotMax + 1);

        int range = maxRange - minRange;
        float curDistance = 0;
        float sumDistance = 0;
        float dist2 = scale;
        int repeatCount = 30;
        Vector3 planetPosition = Vector3.zero;
        bool found = true;
        int satelliteCountMin = int.Parse(template.GetValue("satellites", "min"));
        int satelliteCountMax = int.Parse(template.GetValue("satellites", "max"));
        int satelliteCount = Random.Range(satelliteCountMin, satelliteCountMax + 1);
        List<TemplateNode> satellitesNodes = template.GetNodeList("satellite");
        if (satellitesNodes.Count > 0)
        {
            for (int i = 0; i < satelliteCount; i++)
            {
                TemplateNode satelliteNode = TemplateNode.GetByWeightsList(satellitesNodes);
                int satelliteMaxRange = int.Parse(satelliteNode.GetValue("maxRange"));
                if (this.sateliteMaxDist < satelliteMaxRange)
                {
                    this.sateliteMaxDist = satelliteMaxRange;
                }
            }
        }
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
        allObjects.AddRange(suns);
        allObjects.AddRange(planets);
        //allObjects.AddRange(system.asteroidFields);
        while (repeatCount > 0 && found)
        {
            Vector2 vPosition = UnityEngine.Random.insideUnitCircle * (range);
            Vector2 fPosition = (vPosition.normalized * minRange);
            Vector2 pos = fPosition + vPosition;
            planetPosition = new Vector3(pos.x, 0, pos.y);

            for (int i = 0; i < allObjects.Count; i++)
            {
                SolarObject pl = allObjects[i];
                if (pl == this)
                {
                    continue;
                }
                float dist1 = Vector3.Distance(pl.GetPosition(), sun.GetPosition());
                dist2 = Vector3.Distance(planetPosition, sun.GetPosition());
                curDistance = Mathf.Abs(dist1 - dist2);
                sumDistance = (pl.scale + this.scale) * 6 + (pl.sateliteMaxDist + this.sateliteMaxDist);
                float sunDistance = (this.scale + (sun.scale) * 2) * 3 + (pl.sateliteMaxDist + this.sateliteMaxDist);
                found = false;
                if (curDistance < sumDistance || dist2 < sunDistance)
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
            model = null;
            systemId = -1;
            return;
        }
        if (planets.Count == 0)
        {
            //planetPosition = new Vector2(0, 0, -701000000);
        }

        SetPosition(planetPosition);
        int findId = 0;
        Planet fplanet = planets.Find(f => f.id == findId);
        while (fplanet != null)
        {
            findId++;
            fplanet = planets.Find(f => f.id == findId);
        }

        this.id = findId;
        this.parentSolarObject = sun;
        byte bDir = (byte)UnityEngine.Random.Range(0, 2);
        this.backDir = false;
        if (bDir > 0)
        {
            this.backDir = true;
        }
        this.orbitProgressAdd = Random.Range(0f, 1f);

        SpaceManager.planets.Add(this);
        this.AddSattelites();
    }

    public Planet(Planet planet, string templateName, int minRange = 0, int maxRange = 0)
    {
        this.parentSolarObject = planet;

        galaxyId = planet.galaxyId;
        systemId = planet.systemId;
        StarSystem system = SpaceManager.GetSystemByID(galaxyId, systemId);
        int planetIndex = SpaceManager.planets.IndexOf(planet);
        this.parentSolarObject = SpaceManager.planets[planetIndex];
        this.deptch = (byte)(parentSolarObject.deptch + 1);

        template = TemplateManager.FindTemplate(templateName, "planet");
        model = template.GetValue("model", "patch");
        int scaleMin = int.Parse(template.GetValue("scale", "min"));
        int scaleMax = int.Parse(template.GetValue("scale", "max"));

        this.scale = Random.Range(scaleMin, scaleMax);

        float speedMin = XMLF.FloatVal(template.GetValue("orbitPeriod", "min"));
        float speedMax = XMLF.FloatVal(template.GetValue("orbitPeriod", "max"));

        float speedRotMin = XMLF.FloatVal(template.GetValue("rotateSpeed", "min"));
        float speedRotMax = XMLF.FloatVal(template.GetValue("rotateSpeed", "max"));

        this.orbitPeriod = Random.Range(speedMin, speedMax + 1);
        this.rotateSpeed = Random.Range(speedRotMin, speedRotMax + 1);

        int range = maxRange - minRange;
        float curDistance = 0;
        float sumDistance = 0;
        float dist2 = 1000;
        int repeatCount = 10;
        Vector3 planetPosition = Vector3.zero;
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
        allObjects.AddRange(planets);
        //allObjects.AddRange(system.asteroidFields);
        while (repeatCount > 0 && found)
        {
            Vector2 vPosition = UnityEngine.Random.insideUnitCircle * (range);
            Vector2 fPosition = (vPosition.normalized * minRange);
            Vector2 pos = fPosition + vPosition;
            plpos = new Vector3(pos.x, 0, pos.y);
            planetPosition = plpos + planet.GetPosition();

            for (int i = 0; i < allObjects.Count; i++)
            {
                SolarObject pl = allObjects[i];
                if (pl == this)
                {
                    continue;
                }

                Vector3 plPos = pl.parentSolarObject.GetPosition() + pl.GetPosition();

                float dist1 = Vector3.Distance(planet.GetPosition(), planetPosition);
                dist2 = 0;
                if (pl != planet)
                {
                    dist2 = Vector2.Distance(planet.GetPosition(), plPos);
                }
                curDistance = Mathf.Abs(dist1 - dist2);
                float planetDistance = (pl.scale + this.scale);
                sumDistance = (pl.scale + this.scale) * 1.5f;
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
            model = null;
            systemId = -1;
            return;
        }

        SetPosition(plpos);
        int findId = 0;
        Planet fplanet = planets.Find(f => f.id == findId);

        while (fplanet != null)
        {
            findId++;
            fplanet = planets.Find(f => f.id == findId);
        }

        this.id = findId;
        byte bDir = (byte)UnityEngine.Random.Range(0, 2);
        this.backDir = false;
        if (bDir > 0)
        {
            this.backDir = true;
        }
        this.orbitProgressAdd = Random.Range(0f, 1f);
        SpaceManager.planets.Add(this);
        this.AddSattelites();
    }

    public override void AddSattelites()
    {
        if (this.deptch > 2)
        {
            return;
        }
        int satelliteCountMin = int.Parse(template.GetValue("satellites", "min"));
        int satelliteCountMax = int.Parse(template.GetValue("satellites", "max"));
        int satelliteCount = Random.Range(satelliteCountMin, satelliteCountMax + 1);
        List<TemplateNode> satellitesNodes = template.GetNodeList("satellite");

        if (satellitesNodes.Count > 0)
        {
            for (int i = 0; i < satelliteCount; i++)
            {
                TemplateNode satelliteNode = TemplateNode.GetByWeightsList(satellitesNodes);
                string satelliteTemplateName = satelliteNode.GetValue("template");
                int satelliteMinRange = int.Parse(satelliteNode.GetValue("minRange"));
                int satelliteMaxRange = int.Parse(satelliteNode.GetValue("maxRange"));
                Planet satellitePlanet = new Planet(this, satelliteTemplateName, satelliteMinRange, satelliteMaxRange);
                satellitePlanet.Init();
            }
        }
    }

    public override void OnRender()
    {
        if (Client.localClient.galaxyId == galaxyId && Client.localClient.systemId == systemId)
        {
            if (main == null)
            {
                StarSystem sys = SpaceManager.GetSystemByID(galaxyId, systemId);
                solarController = new GameObject().AddComponent<SolarController>();
                if (parentSolarObject.main == null)
                {
                    parentSolarObject.OnRender();
                }
                if (parentSolarObject.GetType() == typeof(Sun))
                    solarController.transform.SetParent(SpaceManager.solarContainer.transform);
                else
                    solarController.transform.SetParent(parentSolarObject.solarController.transform);
                solarController.transform.localPosition = GetPosition();
                solarController.transform.eulerAngles = GetRotation();
                GameObject sunGameobject = Resources.Load<GameObject>($"{model}/MAIN");
                main = GameObject.Instantiate(sunGameobject, solarController.transform);
                float fscale = scale;
                main.transform.localScale = new Vector3(fscale, fscale, fscale);
                GameObject hull = main.transform.Find("HULL").gameObject;
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
                solarController.gameObject.name = "Planet_" + id.ToString();
                solarController.gameObject.layer = 7;
            }
        }
        else
        {
            Destroy();
        }
    }
}
