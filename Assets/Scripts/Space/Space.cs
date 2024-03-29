using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[Serializable]
public class Space
{
    public int id = -1;
    public int size = 10;
    public string templateName;
    public string name;

    public float[] position = new float[] { 0, 0, 0 };

    public float[] rotation = new float[] { 0, 0, 0 };

    public byte[] color = new byte[] { 255, 255, 255, 255 };

    public SpaceController spaceController;
    public UiSpaceObject uiSpaceObject;

    public Space() { }
    public Space(string templateName)
    {
        this.templateName = templateName;
    }

    public virtual int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.galaxies.Find(f => f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.galaxies.Find(f => f.id == curId);
        }
        id = curId;
        return id;
    }

    public virtual string GetSpaceType()
    {
        string ret = "space";
        if (GetType() == typeof(Galaxy))
        {
            ret = "galaxy";
        }
        else if (GetType() == typeof(StarSystem))
        {
            ret = "system";
        }
        else if (GetType() == typeof(Sector))
        {
            ret = "sector";
        }
        else if (GetType() == typeof(Zone))
        {
            ret = "zone";
        }
        return ret;
    }

    public void Render()
    {
        Vector3 pos = GetPosition();
        Vector3 rot = GetRotation();

        if (GetType() == typeof(Galaxy))
        {
            spaceController = GameObject.Instantiate(GameContentManager.galaxyPrefab, MinimapPanel.galaxiesContainer.transform);
        }
        else if (GetType() == typeof(StarSystem))
        {
            spaceController = GameObject.Instantiate(GameContentManager.systemPrefab, MinimapPanel.systemsContainer.transform);
        }
        else if (GetType() == typeof(Sector))
        {
            pos /= Sector.minimapDivFactor;
            spaceController = GameObject.Instantiate(GameContentManager.sectorPrefab, MinimapPanel.sectorsContainer.transform);
            spaceController.transform.localScale = new Vector3(this.size / Sector.minimapDivFactor, this.size / Sector.minimapDivFactor, this.size / Sector.minimapDivFactor);
        }
        else if (GetType() == typeof(Zone))
        {
            pos /= Zone.minimapDivFactor;
            Zone zn = (Zone)this;
            spaceController = GameObject.Instantiate(GameContentManager.zonePrefab, MinimapPanel.zonesContainer.transform);
            spaceController.transform.localScale = new Vector3(this.size / Zone.minimapDivFactor, this.size / Zone.minimapDivFactor, this.size / Zone.minimapDivFactor);
        }
        spaceController.space = this;
        spaceController.transform.localPosition = pos;
        spaceController.transform.localEulerAngles = rot;
        spaceController.meshRenderer.material.SetColor("_TintColor", GetColor());
        spaceController.meshRenderer.material.SetColor("_Color", GetColor());

        uiSpaceObject = GameObject.Instantiate(GameContentManager.uiSpaceObjectPrefab, CanvasManager.canvas.transform);
        uiSpaceObject.space = this;
        spaceController.uiSpaceObject = uiSpaceObject;
        uiSpaceObject.Init();
    }

    public virtual void SetColor(Color32 color)
    {
        this.color = new byte[] { color.r, color.g, color.b, color.a };
    }
    public virtual Color32 GetColor()
    {
        return new Color32(this.color[0], this.color[1], this.color[2], this.color[3]);
    }
    public virtual void SetPosition(Vector3 position)
    {
        this.position = new float[] { position.x, position.y, position.z };
    }
    public virtual Vector3 GetPosition()
    {
        return new Vector3(this.position[0], this.position[1], this.position[2]);
    }
    public virtual void SetRotation(Vector3 rotation)
    {
        this.rotation = new float[] { rotation.x, rotation.y, rotation.z };
    }
    public virtual Vector3 GetRotation()
    {
        return new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
    }
}