using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : SPObject
{
    public static Pilot Create(string templateName)
    {
        Pilot pilot = Client.localClient.gameObject.AddComponent<Pilot>();
        pilot.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "pilot");
        TemplateNode modelNode = template.GetNode("model");
        pilot.modelPatch = modelNode.GetValue("patch");
        pilot.isPlayerControll = true;
        pilot.Init();

        pilot.rigidbodyMain = pilot.gameObject.AddComponent<Rigidbody>();
        pilot.rigidbodyMain.useGravity = false;
        pilot.rigidbodyMain.angularDrag = 2f;
        pilot.rigidbodyMain.drag = 2f;
        pilot.rigidbodyMain.mass = 10f;

        pilot.controller = pilot.gameObject.AddComponent<PlayerController>();
        pilot.controller.obj = pilot;
        //pilot.controller.maxSpeed = 2000;
        //pilot.controller.velocity = 10000;

        SPObjectManager.singleton.pilots.Add(pilot);
        return pilot;
    }
}
