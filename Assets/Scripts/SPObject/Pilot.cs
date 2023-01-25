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

        TemplateNode paramsNode = template.GetNode("params");
        float scaleMin = XMLF.FloatVal(paramsNode.GetValue("scaleMin"));
        float scaleMax = XMLF.FloatVal(paramsNode.GetValue("scaleMax"));
        float scale = Random.Range(scaleMin, scaleMax + 1);
        if (scale == 0)
        {
            scale = XMLF.FloatVal(paramsNode.GetValue("scale"));
            if (scale == 0){
                scale = 1;
            }
        }
        byte scaleMass = byte.Parse(paramsNode.GetValue("scaleMass"));
        pilot.rigidbodyMain = pilot.gameObject.AddComponent<Rigidbody>();
        pilot.rigidbodyMain.useGravity = false;
        pilot.rigidbodyMain.angularDrag = int.Parse(paramsNode.GetValue("angulardrag"));
        pilot.rigidbodyMain.drag = int.Parse(paramsNode.GetValue("drag"));
        pilot.rigidbodyMain.mass = int.Parse(paramsNode.GetValue("mass"));
        pilot.gameObject.transform.localScale = new Vector3(scale,scale,scale);
        if (scaleMass > 0)
        {
            pilot.rigidbodyMain.mass *= scale;
        }

        return pilot;
    }
}
