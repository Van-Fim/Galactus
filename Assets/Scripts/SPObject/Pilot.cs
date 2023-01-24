using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Pilot : SPObject
{
    [SyncVar]
    public Vector3 syncPosition;
    [SyncVar]
    public Quaternion syncRotation;
    public static Pilot Create(string templateName)
    {
        Pilot pilot = GameObject.Instantiate(GameContentManager.pilotPrefab);
        pilot.templateName = templateName;
        Template template = TemplateManager.FindTemplate(templateName, "pilot");
        TemplateNode modelNode = template.GetNode("model");
        pilot.modelPatch = modelNode.GetValue("patch");
        pilot.isPlayerControll = true;

        return pilot;
    }

    public void FixedUpdate()
    {

    }
    [Command(requiresAuthority = false)]
    void CmdProvidePositionToServer(Vector3 position, Quaternion rotation)
    {
        syncPosition = position;
        syncRotation = rotation;
    }
}
