using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[System.Serializable]
public class TemplateManager
{
    static List<Template> templates;

    public static List<Template> Templates { get => templates; set => templates = value; }

    public static void Func01(XmlNode node, Template template, TemplateNode parentNode = null)
    {
        XmlNodeList childNodes = node.ChildNodes;
        if (template.XmlNodes.IndexOf(node) >= 0)
            return;
        template.XmlNodes.Add(node);
        TemplateNode templateNode = new TemplateNode(template, node.Name, node.InnerText);
        if(parentNode != null)
        {
            templateNode.ParentNode = parentNode;
        }
        if (node.Attributes != null)
        {
            for (int a = 0; a < node.Attributes.Count; a++)
            {
                XmlAttribute xmlAttribute = node.Attributes[a];
                TemplateItem templateItem = new TemplateItem(templateNode, xmlAttribute.Name, xmlAttribute.Value, "");
            }
        }
        else
        {

        }
        for (int i = 0; i < childNodes.Count; i++)
        {
            XmlNode xmlNode = childNodes[i];
            Func01(xmlNode, template, templateNode);
        }
    }
    public static List<Template> LoadTemplates(string node, string patch, bool addToTemplates = true)
    {
        List<Template> ret = new List<Template>();
        TextAsset textAsset = (TextAsset)Resources.Load(patch);
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(textAsset.text);

        if (Templates == null)
            Templates = new List<Template>();

        XmlElement xRoot = xDoc.DocumentElement;
        XmlNodeList nodes = xRoot.SelectNodes("//"+ node);
        if (nodes == null)
            nodes = xRoot.SelectNodes("/" + node);
        if (nodes == null)
            return null;
        
        for (int i = 0; i < nodes.Count; i++)
        {
            Template template = null;
            string templateName = "";
            XmlNode nodeItem = nodes[i];
            
            if (nodeItem.ParentNode != null && nodeItem.ParentNode.Name != "templates")
                continue;
            for (int a = 0; a < nodeItem.Attributes.Count; a++)
            {
                XmlAttribute xmlAttribute = nodeItem.Attributes[a];
                if (xmlAttribute.Name == "name")
                {
                    template = new Template(xmlAttribute.Value, node);
                    templateName = xmlAttribute.Value;
                    break;
                }
            }
            
            Func01(nodeItem, template);
            if (template == null)
                continue;
            XmlNodeList childNodes = nodeItem.ChildNodes;
            for (int j = 0; j < childNodes.Count; j++)
            {
                XmlNode childNode = childNodes[j];

                XmlNodeList childChildNodes = childNode.ChildNodes;
                Func01(childNode, template);
            }
            template.XmlNodes = null;
            if (template != null && addToTemplates)
            {
                Template ftemp = Templates.Find(f => f.TemplateName == template.TemplateName && f.TemplateType == template.TemplateType);
                if (ftemp != null)
                {
                    int ind = Templates.IndexOf(ftemp);
                    Templates.Remove(Templates[ind]);
                }
                Templates.Add(template);
            }
            if (template != null)
                ret.Add(template);
        }
        /*
        for (int i = 0; i < templates.Count; i++)
        {
            Template template = templates[i];
            Debug.Log(template.TemplateType + " " + template.TemplateName);
            for (int j = 0; j < template.TemplateNodes.Count; j++)
            {
                TemplateNode templateNode = template.TemplateNodes[j];
                //Debug.Log(templateNode.Node + " " + templateNode.TemplateItems.Count);
            }
        }
        */
        return ret;
    }
    public static Template FindTemplate(string templateName, string templateType)
    {
       return Templates.Find(f => f.TemplateName == templateName && f.TemplateType == templateType);
    }
    public static List<Template> FindTemplates(string templateType)
    {
        return Templates.FindAll(f => f.TemplateType == templateType);
    }
}
