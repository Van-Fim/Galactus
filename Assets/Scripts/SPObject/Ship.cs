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
        ship.rigidbodyMain = ship.gameObject.AddComponent<Rigidbody>();
        ship.rigidbodyMain.useGravity = false;
        ship.rigidbodyMain.angularDrag = int.Parse(paramsNode.GetValue("angulardrag"));
        ship.rigidbodyMain.drag = int.Parse(paramsNode.GetValue("drag"));
        ship.rigidbodyMain.mass = int.Parse(paramsNode.GetValue("mass"));
        ship.gameObject.transform.localScale = new Vector3(scale,scale,scale);
        if (scaleMass > 0)
        {
            ship.rigidbodyMain.mass *= scale;
        }

        return ship;
    }
}