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

        return pilot;
    }
}
