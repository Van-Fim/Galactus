using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : SPObject
{
    public static Asteroid Create(string templateName)
    {
        Asteroid aster = Instantiate(GameContentManager.asteroidPrefab);
        aster.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "asteroid");
        TemplateNode modelNode = template.GetNode("model");
        aster.modelPatch = modelNode.GetValue("patch");

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
        aster.rigidbodyMain = aster.gameObject.AddComponent<Rigidbody>();
        aster.rigidbodyMain.useGravity = false;
        aster.rigidbodyMain.angularDrag = int.Parse(paramsNode.GetValue("angulardrag"));
        aster.rigidbodyMain.drag = int.Parse(paramsNode.GetValue("drag"));
        aster.rigidbodyMain.mass = int.Parse(paramsNode.GetValue("mass"));
        aster.gameObject.transform.localScale = new Vector3(scale,scale,scale);
        if (scaleMass > 0)
        {
            aster.rigidbodyMain.mass *= scale;
        }

        return aster;
    }
}