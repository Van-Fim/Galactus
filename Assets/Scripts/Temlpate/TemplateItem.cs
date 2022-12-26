using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateItem
{
    string valueName;
    string value;
    string text;
    TemplateNode templateNode;

    public TemplateItem() { }
    public TemplateItem(TemplateNode templateNode, string valueName, string value, string text)
    {
        this.ValueName = valueName;
        value = value.Replace('.', ',');
        this.Value = value;
        this.Text = text;

        templateNode.TemplateItems.Add(this);
    }

    public string ValueName { get => valueName; set => valueName = value; }
    public string Value { get => value; set => this.value = value; }
    public string Text { get => text; set => text = value; }
}

public class TemplateNode
{
    Template template;
    List<TemplateItem> templateItems;
    string node;

    string text;

    TemplateNode parentNode;
    public TemplateNode() { }
    public TemplateNode(Template template, string name, string text)
    {
        templateItems = new List<TemplateItem>();
        this.template = template;
        this.node = name;
        this.text = text;
        template.TemplateNodes.Add(this);
    }

    public Template Template { get => template; set => template = value; }
    public List<TemplateItem> TemplateItems { get => templateItems; set => templateItems = value; }
    public string Node { get => node; set => node = value; }
    public TemplateNode ParentNode { get => parentNode; set => parentNode = value; }
    public string Text { get => text; set => text = value; }

    public static TemplateNode GetByWeightsList(List<TemplateNode> list)
    {
        TemplateNode ret = null;
        int weightSum = 0;
        int min = 0;
        int max = 0;
        List<int> maxValues = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            int wght = int.Parse(list[i].GetValue("weight"));
            if (wght <= 0)
            {
                wght = 10;
            }
            weightSum += wght;
            maxValues.Add(weightSum);
        }
        int rand = Random.Range(0, weightSum);
        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0)
            {
                min = maxValues[i - 1];
            }
            max = maxValues[i];
            if (rand >= min && (rand < max || rand == weightSum))
            {
                ret = list[i];
                return ret;
            }
        }
        return ret;
    }

    public string GetValue(string valueName, bool zeroNull = true)
    {
        string ret = null;
        if (zeroNull)
            ret = "0";
        TemplateItem item = templateItems.Find(f => f.ValueName == valueName);
        if (item == null)
            return ret;
        ret = item.Value;
        return ret;
    }
    public string GetText(string valueName, bool zeroNull = true)
    {
        string ret = null;
        if (zeroNull)
            ret = "0";
        TemplateItem item = templateItems.Find(f => f.ValueName == valueName);
        if (item == null)
            return ret;
        ret = item.Value;
        return ret;
    }
    public List<TemplateNode> GetChildNodesList(string nodeName, string nodeValueName, string nodeValue)
    {
        List<TemplateNode> nodes = Template.TemplateNodes.FindAll(f => f.Node == nodeName && f.GetValue(nodeValueName) == nodeValue && f.ParentNode == this);
        if (nodes == null)
            nodes = new List<TemplateNode>();
        return nodes;
    }
    public TemplateNode GetChildNode(string nodeName)
    {
        List<TemplateNode> nodes = GetChildNodesList(nodeName);
        if (nodes.Count > 0)
            return nodes[0];
        return null;
    }
    public List<TemplateNode> GetChildNodesList(string nodeName)
    {
        List<TemplateNode> nodes = Template.TemplateNodes.FindAll(f => f.Node == nodeName && f.ParentNode == this);
        if (nodes == null)
            nodes = new List<TemplateNode>();
        return nodes;
    }
}
