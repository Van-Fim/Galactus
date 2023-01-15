using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public abstract class Space
{
    public int id = -1;
    public int size = 10;
    public string templateName;
    public string name;

    public float[] position = new float[] { 0, 0, 0 };

    public float[] rotation = new float[] { 0, 0, 0 };

    public byte[] color = new byte[] { 255, 255, 255, 255 };
    public byte[] bgcolor = new byte[] { 255, 255, 255, 255 };

    public SpaceController spaceController;
    public SpaceUiObj uiObj;

    public static UnityAction OnRenderAction;
    public static UnityAction OnDrawUiAction;

    public Space() { }
    public Space(string templateName)
    {
        this.templateName = templateName;
    }
    public virtual void Init()
    {
        OnRenderAction += OnRender;
        OnDrawUiAction += OnDrawUiAction;
    }

    public virtual void DestroyController(int layer)
    {
        if (spaceController != null && spaceController.layer == layer)
        {
            GameObject.DestroyImmediate(spaceController.gameObject);
        }
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

    public virtual void OnRender()
    {

    }

    public virtual void OnDrawUi()
    {
        if (uiObj == null)
        {
            uiObj = GameObject.Instantiate(GameContentManager.spaceUiObjPrefab, CanvasManager.canvas.transform);
            uiObj.space = this;
            uiObj.transform.localPosition = Vector3.zero;
            uiObj.transform.localRotation = Quaternion.identity;
        }
    }

    public virtual void SetColor(Color32 color)
    {
        this.color = new byte[] { color.r, color.g, color.b, color.a };
    }
    public virtual Color32 GetColor()
    {
        return new Color32(this.color[0], this.color[1], this.color[2], this.color[3]);
    }
    public virtual void SetBgColor(Color32 color)
    {
        this.bgcolor = new byte[] { color.r, color.g, color.b, color.a };
    }
    public virtual Color32 GetBgColor()
    {
        return new Color32(this.bgcolor[0], this.bgcolor[1], this.bgcolor[2], this.bgcolor[3]);
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
    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }
    public static void InvokeDrawUi()
    {
        OnDrawUiAction?.Invoke();
    }
}