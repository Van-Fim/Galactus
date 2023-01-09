using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ship : SPObject
{
    public static Ship CreateShip(string templateName)
    {
        Ship ship = Instantiate(GameContentManager.shipPrefab);
        ship.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "ship");
        TemplateNode modelNode = template.GetNode("model");
        ship.modelPatch = modelNode.GetValue("patch");
        ship.Init();
        SpaceObjectNetManager.singleton.ships.Add(ship);
        return ship;
    }
}
