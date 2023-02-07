using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine;

public class SolarObject
{
    public static int scaleFactor = 50000;
    public static int hyperScaleFactor = 1;
    public SolarController solarController;
    public int parentSolarObjectId;
    public int galaxyId;
    public int systemId;
    public int id = 0;
    public string model;
    public int scale;
    public int countSectors = 0;
    public float orbitPeriod;
    public int sateliteMaxDist = 0;
    public byte deptch = 0;

    public bool backDir = false;

    public float[] position = new float[] { 0, 0, 0 };
    public float[] rotation = new float[] { 0, 0, 0 };
    public byte[] orbitColor = new byte[] { 0, 0, 0, 0 };
    public byte[] color = new byte[] { 0, 0, 0, 0 };

    public List<Space> list = new List<Space>();

    public GameObject main;
    public Transform transform;

    [System.NonSerialized]
    public ParticleSystem[] particleSystems;

    [System.NonSerialized]
    public Template template;

    public Space space;

    public static UnityAction OnRenderAction;

    public virtual void Init()
    {
        OnRenderAction += OnRender;
    }

    public virtual void OnRender()
    {
        if (Client.localClient.galaxyId == galaxyId && Client.localClient.systemId == systemId)
        {
            if (main == null)
            {
                StarSystem sys = SpaceManager.GetSystemByID(galaxyId, systemId);
                solarController = new GameObject().AddComponent<SolarController>();
                solarController.transform.SetParent(SpaceManager.solarContainer.transform);
                solarController.transform.localPosition = GetPosition();
                solarController.transform.eulerAngles = GetRotation();
                GameObject sunGameobject = Resources.Load<GameObject>($"{model}/MAIN");
                main = GameObject.Instantiate(sunGameobject, solarController.transform);
                main.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else
        {
            Destroy();
        }
    }

    public virtual void AddSattelites()
    {

    }

    public virtual void DrawCircle(SolarObject parentSolarObject)
    {
        Vector2 position = parentSolarObject.GetPosition();
        Vector2 pos = this.GetPosition();

        float radius = Vector3.Distance(position, pos);
    }

    public virtual Color32 GetColor()
    {
        return new Color32(this.color[0], this.color[1], this.color[2], this.color[3]);
    }
    public virtual void SetColor(Color32 color)
    {
        this.color = new byte[] { color.r, color.g, color.b, color.a };
    }
    public virtual Color32 GetOrbitColor()
    {
        return new Color32(this.orbitColor[0], this.orbitColor[1], this.orbitColor[2], this.orbitColor[3]);
    }
    public virtual void SetOrbitColor(Color32 color)
    {
        this.orbitColor = new byte[] { color.r, color.g, color.b, color.a };
    }
    public virtual void SetPosition(Vector3 position)
    {
        this.position = new float[] { position.x, position.y, position.z };
        // if (solarController != null)
        // {
        //     solarController.transform.localPosition = new Vector3(position.x, position.y, solarController.transform.localPosition.z);
        // }
    }
    public virtual Vector3 GetPosition()
    {
        Vector3 ret = new Vector3(this.position[0], this.position[1], this.position[1]);
        // if(solarController != null) {
        //     SetPosition(solarController.transform.localPosition);
        //     ret = new Vector2(this.position[0], this.position[1]);
        // }
        return ret;
    }
    public virtual void SetRotation(Vector3 rotation)
    {
        this.rotation = new float[] { rotation.x, rotation.y, rotation.z };
    }
    public virtual Vector3 GetRotation()
    {
        return new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
    }

    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }

    public void Destroy()
    {
        if (this.solarController == null)
        {
            return;
        }
        GameObject.DestroyImmediate(this.solarController.gameObject);
    }
}