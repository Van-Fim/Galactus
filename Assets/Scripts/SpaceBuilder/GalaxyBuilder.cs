using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyBuilder : ISpaceBuilder
{
    //--------------------------------------------
    public static double Pow3Constrained(double x)
    {
        double value = Math.Pow(x - 0.5, 3) * 4 + 0.5d;
        return Math.Max(Math.Min(1, value), 0);
    }
    //--------------------------------------------
    public static void GenerateGalaxy(Galaxy galaxy, SpaceManager spm)
    {
        if (galaxy.loaded)
        {
            return;
        }
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
        if (spm is MapSpaceManager)
        {
            for (int i = 0; i < spm.starSystems.Count; i++)
            {
                spm.starSystems[i].Destroy();
            }
            spm.starSystems = new List<StarSystem>();
        }
        string galaxySeed = (GameManager.singleton.seed + "_" + galaxy.id.ToString());
        System.Random r = new System.Random(galaxySeed.GetHashCode());
        UnityEngine.Random.InitState(galaxySeed.GetHashCode());
        int numOfArms = int.Parse(currentGalaxytemplate.GetValue("galaxy", "numOfArms"));
        float spin = XMLF.FloatVal(currentGalaxytemplate.GetValue("galaxy", "spin"));
        float armSpread = XMLF.FloatVal(currentGalaxytemplate.GetValue("galaxy", "armSpread"));
        float starsAtCenterRatio = XMLF.FloatVal(currentGalaxytemplate.GetValue("galaxy", "starsAtCenterRatio"));

        int minCount = int.Parse(currentGalaxytemplate.GetValue("galaxy", "systems_min"));
        int maxCount = int.Parse(currentGalaxytemplate.GetValue("galaxy", "systems_max"));

        int numOfStars = UnityEngine.Random.Range(minCount, maxCount + 1);

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
        if (galaxyType == "galaxy")
        {
            for (int i = 0; i < numOfArms; i++)
            {
                GenerateArm(spm, galaxySeed, galaxy, nodes, numOfStars / numOfArms, (float)i / (float)numOfArms, spin, armSpread, starsAtCenterRatio, galMaxRange);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                GenerateSystem(spm, galaxySeed, galaxy, nodes, galMaxRange);
            }
        }
        galaxy.loaded = true;
        //InitParticle();
        //particleSystem.time = 20;
        //particleSystem.Pause();
    }
    public static void GenerateSystem(SpaceManager spm, string galaxySeed, Galaxy galaxy, List<TemplateNode> nodes, float galMaxRange)
    {
        System.Random r = new System.Random(galaxySeed.GetHashCode());

        TemplateNode node = TemplateNode.GetByWeightsList(nodes);
        int counter = 10;
        List<TemplateNode> starList = new List<TemplateNode>(nodes);

        Vector2 position2D = UnityEngine.Random.insideUnitCircle * galMaxRange;
        int Ymin = int.Parse(node.GetValue("Ymin"));
        int Ymax = int.Parse(node.GetValue("Ymax"));
        int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
        Vector3 position = new Vector3(position2D.x, 0, position2D.y);
        float dst = Vector3.Distance(Vector3.zero, position);
        float minRange = XMLF.FloatVal(node.GetValue("minRange"));
        float maxRange = XMLF.FloatVal(node.GetValue("maxRange"));
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
        Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");
        StarSystem system = new StarSystem(systemTemplateName, galaxy, spm);
        position = new Vector3(position2D.x, yPos, position2D.y);
        system.SetPosition(position);
        system.templateName = systemTemplateName;

        while (counter > 0)
        {
            bool br = false;
            for (int i1 = 0; i1 < spm.starSystems.Count; i1++)
            {
                StarSystem system1 = spm.starSystems[i1];

                if (system1 == system || system1.galaxyId != galaxy.id)
                {
                    continue;
                }
                Vector2 pos1 = system1.GetPosition();
                dst = Vector2.Distance(pos1, position);
                if (dst < 100)
                {
                    position2D = UnityEngine.Random.insideUnitCircle * galMaxRange;
                    position = new Vector3(position2D.x, 0, position2D.y);
                    starList = null;
                    starList = new List<TemplateNode>(nodes);
                    node = TemplateNode.GetByWeightsList(starList);
                    float dst1 = Vector3.Distance(Vector3.zero, position);
                    minRange = XMLF.FloatVal(node.GetValue("minRange"));
                    maxRange = XMLF.FloatVal(node.GetValue("maxRange"));
                    while ((dst1 < minRange || dst1 > maxRange) && starList.Count > 1)
                    {
                        starList.Remove(node);

                        node = TemplateNode.GetByWeightsList(starList);
                        minRange = XMLF.FloatVal(node.GetValue("minRange"));
                        maxRange = XMLF.FloatVal(node.GetValue("maxRange"));
                        Ymin = int.Parse(node.GetValue("Ymin"));
                        Ymax = int.Parse(node.GetValue("Ymax"));
                    }
                    yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                    position = new Vector3(position2D.x, yPos, position2D.y);
                    dst1 = Vector3.Distance(Vector3.zero, position);

                    systemTemplateName = node.GetValue("template");
                    system.templateName = systemTemplateName;
                    system.SetPosition(position);
                    systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");

                    br = true;
                    break;
                }
            }
            if (br)
            {
                counter--;
                if (counter == 0)
                {
                    spm.starSystems.Remove(system);
                }
                continue;
            }
            else
            {
                List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
                TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
                List<TemplateNode> colorbgNodes = systemTemplate.GetNodeList("bg_color");
                TemplateNode colorbgNode = TemplateNode.GetByWeightsList(colorbgNodes);
                List<TemplateNode> skyboxNodes = systemTemplate.GetNodeList("skybox");
                TemplateNode skyboxNode = TemplateNode.GetByWeightsList(skyboxNodes);
                system.templateName = systemTemplateName;
                system.skyboxName = skyboxNode.GetValue("name");
                system.SetPosition(position);
                // int prevSeed = UnityEngine.Random.state.GetHashCode();
                // UnityEngine.Random.InitState($"system_{system.id}_{galaxySeed}".GetHashCode());
                system.SetColor(colorNode.GetColor());
                if (colorbgNode != null)
                {
                    system.SetBgColor(colorbgNode.GetColor());
                }
                system.Init();
                spm.starSystems.Add(system);
                break;
            }
        }
    }
    public static void GenerateArm(SpaceManager spm, string galaxySeed, Galaxy galaxy, List<TemplateNode> nodes, int numOfStars, float rotation, float spin, double armSpread, double starsAtCenterRatio, float galMaxRange)
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
            StarSystem starSystem = new StarSystem(systemTemplateName, galaxy, spm);
            starSystem.SetPosition(galaxyStarPosition);
            Template systemTemplate = TemplateManager.FindTemplate(systemTemplateName, "system");

            List<TemplateNode> colorNodes = systemTemplate.GetNodeList("color");
            TemplateNode colorNode = TemplateNode.GetByWeightsList(colorNodes);
            List<TemplateNode> colorbgNodes = systemTemplate.GetNodeList("bg_color");
            TemplateNode colorbgNode = TemplateNode.GetByWeightsList(colorbgNodes);
            List<TemplateNode> skyboxNodes = systemTemplate.GetNodeList("skybox");
            TemplateNode skyboxNode = TemplateNode.GetByWeightsList(skyboxNodes);

            starSystem.templateName = systemTemplateName;
            starSystem.skyboxName = skyboxNode.GetValue("name");

            starSystem.SetColor(colorNode.GetColor());
            if (colorbgNode != null)
            {
                starSystem.SetBgColor(colorbgNode.GetColor());
            }
            // UnityEngine.Random.InitState(prevSeed);
            starSystem.Init();
            spm.starSystems.Add(starSystem);
        }
    }
    public static void Build(SpaceManager spm, Galaxy galaxy)
    {
        GenerateGalaxy(galaxy, spm);
    }
}
