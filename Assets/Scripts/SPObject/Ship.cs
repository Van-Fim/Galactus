using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : SPObject
{
    public static Ship Create(string templateName)
    {
        Ship ship = Instantiate(GameContentManager.shipPrefab);
        ship.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "ship");
        TemplateNode modelNode = template.GetNode("model");
        ship.modelPatch = modelNode.GetValue("patch");
        ship.Init();
        SPObjectManager.singleton.ships.Add(ship);
        return ship;
    }
}