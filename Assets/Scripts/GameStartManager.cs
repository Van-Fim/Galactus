using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager
{
    public static List<GameStartData> LoadGameStarts()
    {
        List<GameStartData> list = ServerDataManager.singleton.ServerData.gameStarts;
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
        int startId = 0;
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
            int galaxyId = int.Parse(spaceNode.GetValue("galaxy"));
            int systemId = int.Parse(spaceNode.GetValue("system"));
            int sectorId = int.Parse(spaceNode.GetValue("sector"));
            int zoneId = int.Parse(spaceNode.GetValue("zone"));
            for (int j = 0; j < shipNodes.Count; j++)
            {
                TemplateNode shipNode = shipNodes[j];
                bool plyShipExist = System.Convert.ToBoolean(byte.Parse(shipNode.GetValue("playerShip")));
                string templateStringName = shipNode.GetValue("template");
                TemplateNode positionNode = shipNode.GetChildNode("position");
                Vector3 position = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                if (positionNode != null)
                {
                    int x = int.Parse(positionNode.GetValue("x"));
                    int y = int.Parse(positionNode.GetValue("y"));
                    int z = int.Parse(positionNode.GetValue("z"));
                    position = new Vector3(x, y, z);
                    if (plyShipExist)
                    {
                        ret.SetPosition(position);
                    }
                }
                TemplateNode rotationNode = shipNode.GetChildNode("rotation");
                if (rotationNode != null)
                {
                    int x = int.Parse(rotationNode.GetValue("x"));
                    int y = int.Parse(rotationNode.GetValue("y"));
                    int z = int.Parse(rotationNode.GetValue("z"));
                    rotation = new Vector3(x, y, z);
                    if (plyShipExist)
                    {
                        ret.SetRotation(rotation);
                    }
                }
                if (plyShipExist)
                {
                    ret.startType = "ship";
                }
                Template tmp = TemplateManager.FindTemplate(templateStringName, "ship");
                TemplateNode loadoutsNode = shipNode.GetChildNode("loadouts");
                TemplateNode hardpointsNode = shipNode.GetChildNode("hardpoints");
                if (loadoutsNode == null)
                {
                    loadoutsNode = tmp.GetNode("loadouts");
                }
                if (hardpointsNode == null)
                {
                    hardpointsNode = tmp.GetNode("hardpoints");
                }
                Data.SpaceObjectData spaceObjectData = new Data.SpaceObjectData();
                spaceObjectData.isStartObject = plyShipExist;
                spaceObjectData.templateName = templateStringName;
                spaceObjectData.SetPosition(position);
                spaceObjectData.SetRotation(rotation);
                spaceObjectData.galaxyId = galaxyId;
                spaceObjectData.systemId = systemId;
                spaceObjectData.sectorId = sectorId;
                spaceObjectData.zoneId = zoneId;
                spaceObjectData.id = (uint)ret.spaceObjectDatas.Count;
                spaceObjectData.type = ret.startType;
                if (plyShipExist)
                {
                    startId = (int)spaceObjectData.id;
                }
                ret.spaceObjectDatas.Add(spaceObjectData);
            }
            if (playerNodes.Count > 0)
            {
                ret.startType = "pilot";
                for (int j = 0; j < playerNodes.Count; j++)
                {
                    TemplateNode playerNode = playerNodes[j];
                    string templateStringName = playerNode.GetValue("template");
                    TemplateNode positionNode = playerNode.GetChildNode("position");
                    Vector3 position = Vector3.zero;
                    Vector3 rotation = Vector3.zero;
                    if (positionNode != null)
                    {
                        int x = int.Parse(positionNode.GetValue("x"));
                        int y = int.Parse(positionNode.GetValue("y"));
                        int z = int.Parse(positionNode.GetValue("z"));
                        position = new Vector3(x, y, z);
                        ret.SetPosition(position);
                    }
                    TemplateNode rotationNode = playerNode.GetChildNode("rotation");
                    if (rotationNode != null)
                    {
                        int x = int.Parse(rotationNode.GetValue("x"));
                        int y = int.Parse(rotationNode.GetValue("y"));
                        int z = int.Parse(rotationNode.GetValue("z"));
                        rotation = new Vector3(x, y, z);
                        ret.SetRotation(rotation);
                    }
                    Template tmp = TemplateManager.FindTemplate(templateStringName, "pilot");
                    TemplateNode loadoutsNode = playerNode.GetChildNode("loadouts");
                    TemplateNode hardpointsNode = playerNode.GetChildNode("hardpoints");
                    if (loadoutsNode == null)
                    {
                        loadoutsNode = tmp.GetNode("loadouts");
                    }
                    if (hardpointsNode == null)
                    {
                        hardpointsNode = tmp.GetNode("hardpoints");
                    }
                    Data.SpaceObjectData spaceObjectData = new Data.SpaceObjectData();
                    spaceObjectData.isStartObject = true;
                    spaceObjectData.templateName = templateStringName;
                    spaceObjectData.SetPosition(position);
                    spaceObjectData.SetRotation(rotation);
                    spaceObjectData.galaxyId = galaxyId;
                    spaceObjectData.systemId = systemId;
                    spaceObjectData.sectorId = sectorId;
                    spaceObjectData.zoneId = zoneId;
                    spaceObjectData.type = ret.startType;
                    spaceObjectData.id = (uint)ret.spaceObjectDatas.Count;
                    ret.spaceObjectDatas.Add(spaceObjectData);
                    startId = (int)spaceObjectData.id;
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
        for (int i = 0; i < ret.spaceObjectDatas.Count; i++)
        {
            if (startId != ret.spaceObjectDatas[i].id)
            {
                ret.spaceObjectDatas[i].isStartObject = false;
            }
        }
        return ret;
    }
    public static GameStartData GetGameStart(string name)
    {
        return ServerDataManager.singleton.ServerData.gameStarts.Find(f => f.templateName == name);
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
