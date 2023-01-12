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

        spaceController = GameObject.Instantiate(GameContentManager.galaxyPrefab);
    }
}
