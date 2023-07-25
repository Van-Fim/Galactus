using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager
{
    public static List<GameStartData> LoadGameStarts()
    {
        List<GameStartData> list = ServerDataManager.singleton.serverData.gameStarts;
        List<Template> templates = TemplateManager.FindTemplates("start");
        for (int i = 0; i < templates.Count; i++)
        {
            Template template = templates[i];
            string name = template.GetValue("start", "name");
            if (name.Length > 0)
            {
                GameStartData gameStart = list.Find(f => f.templateName == name);
                if (gameStart == null)
                {
                    gameStart = LoadGameStart(name);
                    list.Add(gameStart);
                }
                else
                {
                    int ind = list.IndexOf(gameStart);
                    list[ind] = LoadGameStart(name);
                }
            }
        }
        return list;
    }
    public static GameStartData LoadGameStart(string name)
    {
        GameStartData ret = null;
        Template template = TemplateManager.FindTemplate(name, "start");
        if (template == null)
        {
            Debug.LogError("Error template not found");
            return ret;
        }

        if (template == null)
        {
            Debug.LogError("Template " + name + " not found");
            return ret;
        }
        List<TemplateNode> spaceNodes = template.GetNodeList("space");
        List<TemplateNode> paramNodes = template.GetNodeList("param");
        List<TemplateNode> resourceNodes = template.GetNodeList("resource");
        ret = new GameStartData();
        ret.name = template.GetValue("start", "text");
        ret.templateName = template.GetValue("start", "name");
        for (int i = 0; i < paramNodes.Count; i++)
        {
            TemplateNode paramNode = paramNodes[i];
            string pName = paramNode.GetValue("name");
            string pValue = paramNode.GetValue("value");

            ret.paramDatas.Add(new Data.ParamData(pName, pValue));
        }
        for (int i = 0; i < resourceNodes.Count; i++)
        {
            TemplateNode resourceNode = resourceNodes[i];
            string pName = resourceNode.GetValue("name");
            string pValue = resourceNode.GetValue("value");

            ret.resourceDatas.Add(new Data.ParamData(pName, pValue));
        }

        for (int i = 0; i < spaceNodes.Count; i++)
        {
            TemplateNode spaceNode = spaceNodes[i];
            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            List<TemplateNode> playerNodes = template.GetNodeList("player");

            for (int j = 0; j < shipNodes.Count; j++)
            {
                TemplateNode shipNode = shipNodes[j];
                bool plyShipExist = System.Convert.ToBoolean(byte.Parse(shipNode.GetValue("playerShip")));
                string templateStringName = shipNode.GetValue("template");
                TemplateNode positionNode = shipNode.GetChildNode("position");
                if (positionNode != null)
                {
                    int x = int.Parse(positionNode.GetValue("x"));
                    int y = int.Parse(positionNode.GetValue("y"));
                    int z = int.Parse(positionNode.GetValue("z"));
                    ret.SetPosition(new Vector3(x, y, z));
                }
                TemplateNode rotationNode = shipNode.GetChildNode("rotation");
                if (rotationNode != null)
                {
                    int x = int.Parse(rotationNode.GetValue("x"));
                    int y = int.Parse(rotationNode.GetValue("y"));
                    int z = int.Parse(rotationNode.GetValue("z"));
                    ret.SetRotation(new Vector3(x, y, z));
                }
                if (plyShipExist)
                {
                    ret.startType = "ship";
                }
            }
            if (playerNodes.Count > 0)
            {
                ret.startType = "pilot";
                for (int j = 0; j < playerNodes.Count; j++)
                {
                    TemplateNode playerNode = playerNodes[j];
                    string templateStringName = playerNode.GetValue("template");
                    TemplateNode positionNode = playerNode.GetChildNode("position");
                    if (positionNode != null)
                    {
                        int x = int.Parse(positionNode.GetValue("x"));
                        int y = int.Parse(positionNode.GetValue("y"));
                        int z = int.Parse(positionNode.GetValue("z"));
                        ret.SetPosition(new Vector3(x, y, z));
                    }
                    TemplateNode rotationNode = playerNode.GetChildNode("rotation");
                    if (rotationNode != null)
                    {
                        int x = int.Parse(rotationNode.GetValue("x"));
                        int y = int.Parse(rotationNode.GetValue("y"));
                        int z = int.Parse(rotationNode.GetValue("z"));
                        ret.SetRotation(new Vector3(x, y, z));
                    }
                }
            }
            if (ret.startType != null)
            {
                ret.paramDatas.Add(new Data.ParamData("galaxy", spaceNode.GetValue("galaxy")));
                ret.paramDatas.Add(new Data.ParamData("system", spaceNode.GetValue("system")));
                ret.paramDatas.Add(new Data.ParamData("sector", spaceNode.GetValue("sector")));
                ret.paramDatas.Add(new Data.ParamData("zone", spaceNode.GetValue("zone")));
            }
        }
        return ret;
    }
    public static GameStartData GetGameStart(string name)
    {
        return ServerDataManager.singleton.serverData.gameStarts.Find(f=>f.templateName == name);
    }
    public static Data.ParamData GetParam(GameStartData gameStartData, string name)
    {
        Data.ParamData ret = null;
        int num = -1;
        Data.ParamData fnd = gameStartData.paramDatas.Find(f => f.name == name);
        if (fnd != null)
        {
            num = gameStartData.paramDatas.IndexOf(fnd);
            if (num >= 0)
            {
                ret = gameStartData.paramDatas[num];
            }
        }
        return ret;
    }
}
