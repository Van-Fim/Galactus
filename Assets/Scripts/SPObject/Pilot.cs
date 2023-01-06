using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pilot : SPObject
{
    public static Pilot InitPlayer(GameObject gameObject, string templateName)
    {
        Pilot pilot = gameObject.GetComponent<Pilot>();
        pilot.enabled = true;
        pilot.isPlayerControll = true;
        pilot.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "pilot");
        TemplateNode modelNode = template.GetNode("model");
        pilot.modelPatch = modelNode.GetValue("patch");
        pilot.Init();

        pilot.rigidbodyMain = gameObject.AddComponent<Rigidbody>();
        pilot.rigidbodyMain.useGravity = false;
        pilot.rigidbodyMain.angularDrag =2f;
        pilot.rigidbodyMain.drag = 2f;

        pilot.controller = gameObject.AddComponent<PlayerController>();
        pilot.controller.obj = pilot;

        SpaceObjectNetManager.singleton.pilots.Add(pilot);
        return pilot;
    }

    public override void OnStartClient()
    {
        if (isServer)
        {

        }
        if (isClient)
        {
            
        }
    }
}
