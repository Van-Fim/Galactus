using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameContent
{
    public class UniverseBuilder : ISpaceBuilder
    {
        public static string templateName = "default";
        public static void Build(SpaceManager spm)
        {
            System.Random rndm = new System.Random(GameManager.GetSeed());
            UnityEngine.Random.InitState(GameManager.GetSeed());
            Template currentUniversetemplate = TemplateManager.FindTemplate(templateName, "universe");
            DebugConsole.ShowErrorIsNull(currentUniversetemplate, "Universe template " + templateName + " is not found", true);

            List<TemplateNode> nodes = currentUniversetemplate.GetNodeList("galaxy");
            TemplateNode nd = currentUniversetemplate.GetNode("galaxies");
            int maxRangeMin = int.Parse(nd.GetValue("maxRangeMin"));
            int maxRangeMax = int.Parse(nd.GetValue("maxRangeMax"));
            float range = UnityEngine.Random.Range(maxRangeMin, maxRangeMax + 1);
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
                        Color32 col = colorNode.GetColor();
                        galaxy.SetColor(col);
                    }
                    int counter = 10;
                    Vector3 position3D = UnityEngine.Random.insideUnitSphere * range;
                    int yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                    Vector3 position = position3D;
                    while (counter > 0)
                    {
                        bool br = false;
                        for (int i1 = 0; i1 < spm.galaxies.Count; i1++)
                        {
                            Galaxy galaxy1 = spm.galaxies[i1];

                            if (galaxy1 == galaxy)
                            {
                                continue;
                            }
                            Vector3 pos1 = galaxy1.GetPosition();
                            float dst = Vector3.Distance(pos1, position);
                            if (dst < size)
                            {
                                position3D = UnityEngine.Random.insideUnitCircle * range;
                                yPos = UnityEngine.Random.Range(Ymin, Ymax + 1);
                                position = position3D;
                                br = true;
                                break;
                            }
                        }
                        if (br)
                        {
                            counter--;
                            if (counter == 0)
                            {
                                spm.galaxies.Remove(galaxy);
                            }
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    galaxy.SetPosition(position);
                    galaxy.Init();
                    spm.galaxies.Add(galaxy);
                }
            }
        }
    }
}