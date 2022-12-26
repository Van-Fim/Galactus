using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[System.Serializable]
public class Template
{
    string templateName;
    string templateType;
    byte templateSubType;

    List<TemplateNode> templateNodes;
    List<XmlNode> xmlNodes;
    public Template(string templateName, string templateType="null", byte templateSubType = 0)
    {
        this.templateNodes = new List<TemplateNode>();
        this.XmlNodes = new List<XmlNode>();
        this.templateName = templateName;
        this.templateType = templateType;
        this.templateSubType = templateSubType;
    }

    public string TemplateName { get => templateName; set => templateName = value; }
    public string TemplateType { get => templateType; set => templateType = value; }
    public List<TemplateNode> TemplateNodes { get => templateNodes; set => templateNodes = value; }
    public List<XmlNode> XmlNodes { get => xmlNodes; set => xmlNodes = value; }

    public string GetValue(string nodeName, string valueName)
    {
        string ret = "0";
        TemplateNode node = templateNodes.Find(f => f.Node == nodeName);
        if (node == null)
            return ret;
        TemplateItem templateItem = node.TemplateItems.Find(f => f.ValueName == valueName);
        if (templateItem == null)
            return ret;
        ret = templateItem.Value;
        return ret;
    }
    public List<TemplateNode> GetNodeList(string nodeName)
    {
        List<TemplateNode> nodes = templateNodes.FindAll(f => f.Node == nodeName);
        if (nodes == null)
            nodes = new List<TemplateNode>();
        return nodes;
    }

    public TemplateNode GetNode(string nodeName, string nodeValueName, string nodeValue)
    {
        TemplateNode node = templateNodes.Find(f => f.Node == nodeName && f.GetValue(nodeValueName) == nodeValue);
        return node;
    }

    public TemplateNode GetNode(string nodeName)
    {
        TemplateNode node = templateNodes.Find(f => f.Node == nodeName);
        return node;
    }
}
