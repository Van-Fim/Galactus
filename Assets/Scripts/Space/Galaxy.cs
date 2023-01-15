using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : Space
{
    public Galaxy() : base() { }
    public Galaxy(string templateName) : base(templateName)
    {
    }
    public override void OnRender()
    {
        Vector3 pos = GetPosition();
        Vector3 rot = GetRotation();
        if (MapClientPanel.currentLayer == GameContentManager.galaxyPrefab.layer)
        {
            if (spaceController == null)
            {
                spaceController = GameObject.Instantiate(GameContentManager.galaxyPrefab, SpaceManager.singleton.transform);
                spaceController.transform.localPosition = pos;
                spaceController.transform.localEulerAngles = rot;
                spaceController.meshRenderer.material.SetColor("_TintColor", GetColor());
                spaceController.meshRenderer.material.SetColor("_Color", GetColor());
                spaceController.meshRenderer.enabled = true;
                spaceController.Init();
            }
        }
        else
        {
            DestroyController(0);
        }
    }

    public override void OnDrawUi()
    {
        base.OnDrawUi();
    }
}
