using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
[System.Serializable]
public class ResourcesManager
{
    public List<ResourcesData> resources = new List<ResourcesData>();

    public ResourcesData GetResource(string name, string subtype)
    {
        ResourcesData ret = null;
        Template temp = TemplateManager.FindTemplate(name, "resource_type");
        string tempSubtype = temp.GetValue("resource_type", "subtype");
        if (tempSubtype != subtype)
        {
            return ret;
        }
        ret = resources.Find(f => f.name == name && f.subType == subtype);
        if (ret == null && temp != null)
        {
            ret = AddResource(temp.TemplateName, temp.GetValue("resource_type", "text"), subtype);
        }
        return ret;
    }
    public void SetResourceValue(string name, string subtype, float value)
    {
        ResourcesData ret = GetResource(name, subtype);
        int ind = resources.IndexOf(ret);
        resources[ind].value = value;
        return;
    }
    public ResourcesData AddResource(string templateName, string name, string subtype, float value = 0)
    {
        ResourcesData ret = null;
        ret = resources.Find(f => f.name == name);
        if (ret == null)
        {
            int curId = 0;
            ResourcesData fnd = resources.Find(f => f.id == curId);

            while (fnd != null)
            {
                curId++;
                fnd = resources.Find(f => f.id == curId);
            }

            ret = new ResourcesData();
            ret.templateName = templateName;
            ret.id = curId;
            ret.name = name;
            ret.value = value;
            ret.subType = subtype;
            resources.Add(ret);
        }
        return ret;
    }
}
