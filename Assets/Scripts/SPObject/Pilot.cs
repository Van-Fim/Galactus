using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Pilot : SPObject
{
    public static Pilot Create(string templateName)
    {
        Pilot pilot = GameObject.Instantiate(GameContentManager.pilotPrefab);
        pilot.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "pilot");
        TemplateNode modelNode = template.GetNode("model");
        pilot.modelPatch = modelNode.GetValue("patch");
        pilot.isPlayerControll = true;
        pilot.Init();

        NetworkServer.Spawn(pilot.gameObject);
        SPObjectManager.singleton.pilotsIds.Add(pilot.netId);
        //pilot.controller.maxSpeed = 2000;
        //pilot.controller.velocity = 10000;

        return pilot;
    }
}
