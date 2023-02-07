using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StarSystem : Space
{
    public int galaxyId;
    public string skyboxName;
    public StarSystem() : base() { }

    public StarSystem(string templateName) : base(templateName)
    {
    }
    public StarSystem(string templateName, Galaxy galaxy) : base(templateName)
    {
        this.galaxyId = galaxy.id;
    }

    public void LoadAsteroids(int chance, int countMin, int countMax)
    {
        int chr = Random.Range(0, 101);
        if (chr <= chance)
        {
            int count = Random.Range(countMin, countMax + 1);
            for (int i = 0; i < count; i++)
            {
                Asteroid ast = Asteroid.Create("Aster01");
                ast.SetSpace(this);
                Vector3 pos = Random.insideUnitSphere * 1000000;
                ast.transform.localPosition = pos;
                NetworkServer.Spawn(ast.gameObject);
            }
        }
    }

    public void LoadSuns()
    {
        Template template = TemplateManager.FindTemplate(templateName, "system");
        if (template == null)
        {
            Debug.LogError("System template " + templateName + " not found");
            return;
        }

        List<TemplateNode> sunNodes = template.GetNodeList("sun");
        if (sunNodes.Count > 0)
        {
            int minCount = int.Parse(template.GetValue("suns", "min"));
            int maxCount = int.Parse(template.GetValue("suns", "max"));
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                TemplateNode sunNode = TemplateNode.GetByWeightsList(sunNodes);
                string templateSName = sunNode.GetValue("template");
                int minRange = int.Parse(sunNode.GetValue("minRange"));
                int maxRange = int.Parse(sunNode.GetValue("maxRange"));
                Sun sun = new Sun(this, templateSName, minRange, maxRange);
                sun.galaxyId = galaxyId;
                sun.systemId = id;
                sun.Init();
            }
        }
    }
    public void LoadPlanets()
    {
        Template template = TemplateManager.FindTemplate(templateName, "system");
        if (template == null)
        {
            Debug.LogError("System template " + templateName + " not found");
            return;
        }
        List<Sun> suns = SpaceManager.suns.FindAll(f => f.galaxyId == galaxyId && f.systemId == id);
        Sun sun = suns[Random.Range(0,suns.Count)];
        List<TemplateNode> planetNodes = template.GetNodeList("planet");
        if (planetNodes.Count > 0)
        {
            int minCount = int.Parse(template.GetValue("planets", "min"));
            int maxCount = int.Parse(template.GetValue("planets", "max"));
            int count = UnityEngine.Random.Range(minCount, maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                TemplateNode planetNode = TemplateNode.GetByWeightsList(planetNodes);
                string templateSName = planetNode.GetValue("template");
                int minRange = int.Parse(planetNode.GetValue("minRange"));
                int maxRange = int.Parse(planetNode.GetValue("maxRange"));
                Planet planet = new Planet(this, sun, templateSName, minRange, maxRange);
                planet.Init();
            }
        }
    }
    public override int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == curId);
        }
        id = curId;
        return id;
    }
    public override void OnRender()
    {
        Vector3 pos = GetPosition();
        Vector3 rot = GetRotation();

        if (MapClientPanel.currentLayer == GameContentManager.systemPrefab.layer && MapClientPanel.selectedGalaxyId == galaxyId)
        {
            spaceController = GameObject.Instantiate(GameContentManager.systemPrefab, SpaceManager.singleton.transform);
            spaceController.transform.localPosition = pos;
            spaceController.transform.localEulerAngles = rot;
            spaceController.meshRenderer.material.SetColor("_TintColor", GetColor());
            spaceController.meshRenderer.material.SetColor("_Color", GetColor());
            spaceController.meshRenderer.enabled = true;
            spaceController.Init();
        }
        else
        {
            DestroyController(1);
        }
    }
    public override void OnDrawUi()
    {
        base.OnDrawUi();
    }
}
