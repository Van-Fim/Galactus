using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HardpointType
{
    public static List<HardpointType> hardpointTypes = new List<HardpointType>();
    public string name;
    public List<string> compatibilities = new List<string>();
    public HardpointType FindHardpointType(string typeName)
    {
        HardpointType ret = null;
        ret = hardpointTypes.Find(f => f.name == typeName);
        return ret;
    }
    public bool CheckHardpointCompType(string typeName)
    {
        bool ret = false;
        string fnd = compatibilities.Find(f => f == typeName);
        if (fnd.Length > 0)
        {
            ret = true;
        }
        return ret;
    }
    public static void LoadAllTypes()
    {
        List<Template> templates = TemplateManager.FindTemplates("hardpoint_type");
        for (int i = 0; i < templates.Count; i++)
        {
            Template template = templates[i];
            string tempName = template.GetValue("hardpoint_type", "name");
            HardpointType hardpointType = new HardpointType();
            hardpointType.name = tempName;
            List<TemplateNode> nodes = template.GetNodeList("hardpoint_compatibility");
            for (int j = 0; j < nodes.Count; j++)
            {
                TemplateNode node = nodes[j];
                string compName = node.GetValue("name");
                hardpointType.compatibilities.Add(compName);
            }
            hardpointTypes.Add(hardpointType);
        }
    }
}
