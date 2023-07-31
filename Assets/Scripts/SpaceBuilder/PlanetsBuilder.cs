using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetsBuilder : ISpaceBuilder
{
    public static void Build(SpaceManager spm, StarSystem starSystem)
    {
        LoadSuns(spm, starSystem);
        LoadPlanets(spm, starSystem);
    }
    public static void LoadSuns(SpaceManager spm, StarSystem starSystem)
    {
        Template template = TemplateManager.FindTemplate(starSystem.templateName, "system");
        if (template == null)
        {
            Debug.LogError("System template " + starSystem.templateName + " not found");
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
                Sun sun = new Sun(spm, starSystem, templateSName, minRange, maxRange);
                sun.galaxyId = starSystem.galaxyId;
                sun.systemId = starSystem.id;
                if (spm is MapSpaceManager)
                {
                    sun.InitMinimap();
                }
                else
                {
                    sun.Init();
                }
            }
        }
    }
    public static void LoadPlanets(SpaceManager spm, StarSystem starSystem)
    {
        Template template = TemplateManager.FindTemplate(starSystem.templateName, "system");
        if (template == null)
        {
            Debug.LogError("System template " + starSystem.templateName + " not found");
            return;
        }
        List<Sun> suns = spm.suns.FindAll(f => f.galaxyId == starSystem.galaxyId && f.systemId == starSystem.id);
        Sun sun = suns[Random.Range(0, suns.Count)];
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
                Planet planet = new Planet(spm, starSystem, sun, templateSName, minRange, maxRange);
                planet.Init();
            }
        }
    }
}
