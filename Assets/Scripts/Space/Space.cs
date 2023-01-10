using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public static Vector3 RecalcPos(Vector3 position, float stepValue)
    {
        Vector3 ret = new Vector3();
        Vector3 c1 = position;
        position = new Vector3(position.x + stepValue / 2, position.y + stepValue / 2, position.z + stepValue / 2);
        ret = new Vector3((int)(position.x / stepValue), (int)(position.y / stepValue), (int)(position.z / stepValue));
        if (c1.x + stepValue / 2 < 0)
        {
            ret.x -= 1;
        }
        if (c1.y + stepValue / 2 < 0)
        {
            ret.y -= 1;
        }
        if (c1.z + stepValue / 2 < 0)
        {
            ret.z -= 1;
        }
        return ret * stepValue;
    }
}