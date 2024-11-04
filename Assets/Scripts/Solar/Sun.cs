using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class Sun : SolarObject
{
    public Light sunLight;
    public static UnityAction OnFixLightDirAction;
    public Sun(SpaceSystem system, string templateName, int minRange = 0, int maxRange = 0)
    {
        Color32 color = new Color32(system.color[0], system.color[1], system.color[2], system.color[3]);
        this.SetColor(color);
        this.galaxyId = system.galaxyId;
        this.systemId = system.id;
        template = TemplateManager.FindTemplate(templateName, "sun");
        model = template.GetValue("model", "patch");
        int scaleMin = int.Parse(template.GetValue("scale", "min"));
        int scaleMax = int.Parse(template.GetValue("scale", "max"));
        this.scale = Random.Range(scaleMin, scaleMax);

        int range = maxRange - minRange;
        float curDistance = 0;
        float sumDistance = 0;
        float dist2 = scale;
        int repeatCount = 30;
        Vector2 centerPosition = Vector3.zero;
        Vector2 sunPosition = Vector3.zero;
        bool found = true;
        List<Sun> allSystemSuns = SpaceManager.suns.FindAll(f => f.galaxyId == system.galaxyId && f.id == system.id);
        while (repeatCount > 0 && found)
        {
            Vector2 vPosition = UnityEngine.Random.insideUnitCircle * (range);
            Vector2 fPosition = (vPosition.normalized * minRange);
            Vector2 pos = fPosition + vPosition;
            sunPosition = new Vector3(pos.x, 0, pos.y);
            allSystemSuns = SpaceManager.suns.FindAll(f => f.galaxyId == system.galaxyId && f.id == system.id);
            if (allSystemSuns.Count == 0)
            {
                found = false;
                break;
            }
            for (int i = 0; i < allSystemSuns.Count; i++)
            {
                Sun sn = allSystemSuns[i];
                if (sn == this)
                {
                    continue;
                }
                float dist1 = Vector2.Distance(sn.GetPosition(), centerPosition);
                dist2 = Vector2.Distance(sunPosition, centerPosition);
                curDistance = Mathf.Abs(dist1 - dist2);
                sumDistance = (sn.scale + this.scale) * 6;
                found = false;

                if (curDistance < sumDistance)
                {
                    //found = true;
                    //break;
                }
            }
            if (!found)
            {
                break;
            }
            repeatCount--;
        }
        if (found)
        {
            return;
        }
        SetPosition(sunPosition);
        int findId = 0;
        Sun fsun = allSystemSuns.Find(f => f.id == findId);
        while (fsun != null)
        {
            findId++;
            fsun = allSystemSuns.Find(f => f.id == findId);
        }
        this.id = findId;
        SpaceManager.suns.Add(this);
        AddSectors();
    }
    public override void Init()
    {
        base.Init();
    }
    public void OnFixLightDir()
    {

    }
    public override void OnRenderMinimap()
    {
        RenderAct();
    }
    public override void RenderAct()
    {
        if ((LocalClient.galaxyId == galaxyId && LocalClient.systemId == systemId))
        {
            if (main == null)
            {
                SpaceSystem sys = SpaceManager.spaceSystems.Find(x => x.galaxyId == galaxyId && x.id == systemId);
                solarController = new GameObject().AddComponent<SolarController>();
                solarController.transform.SetParent(SpaceManager.solarContainer.transform);
                solarController.transform.localPosition = GetPosition();
                solarController.transform.eulerAngles = GetRotation();
                solarController.solarObject = this;
                GameObject sunGameobject = Resources.Load<GameObject>($"{model}/MAIN");
                main = GameObject.Instantiate(sunGameobject, solarController.transform);
                float fscale = scale;
                solarController.gameObject.layer = 7;
                GameObject hull = main.transform.Find("HULL").gameObject;
                sunLight = GameObject.Instantiate(GamePrefabsManager.LoadPrefab<Light>("SunLightPrefab"));

                main.transform.localScale = new Vector3(fscale, fscale, fscale);

                Color32 col = sys.GetColor();

                hull.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", col);
                hull.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", col);
                hull.GetComponent<MeshRenderer>().material.SetColor("_TintColor", col);


                sunLight.transform.SetParent(main.transform);
                sunLight.transform.localPosition = Vector3.zero;
                sunLight.color = col;

                solarController.gameObject.name = "Sun_" + id.ToString();

            }
            else
            {
                if (!CameraManager.mainCamera.gameObject.activeSelf && CameraManager.mapCamera.gameObject.activeSelf)
                {
                    main.gameObject.SetActive(false);
                }
                else
                {
                    main.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            Destroy();
        }
    }
    public void AddSectors()
    {
        if (this.deptch > 2)
        {
            return;
        }
        int sectorCountMin = int.Parse(template.GetValue("sectors", "min"));
        int sectorCountMax = int.Parse(template.GetValue("sectors", "max"));
        int sectorCount = Random.Range(sectorCountMin, sectorCountMax + 1);
        List<TemplateNode> sectorsNodes = template.GetNodeList("sector");

        if (sectorsNodes.Count > 0)
        {
            for (int i = 0; i < sectorCount; i++)
            {
                TemplateNode sectorNode = TemplateNode.GetByWeightsList(sectorsNodes);
                string sectorTemplateName = sectorNode.GetValue("template");
                int sectorMinRange = int.Parse(sectorNode.GetValue("minRange"));
                int sectorMaxRange = int.Parse(sectorNode.GetValue("maxRange"));
                Sector sector = new Sector(this, sectorTemplateName, sectorMinRange, sectorMaxRange);
                sector.Init();
            }
        }
    }
    public int GenerateId()
    {
        int curId = 0;
        SpaceSystem sys = SpaceManager.spaceSystems.Find(f => f.galaxyId == galaxyId && f.id == systemId && f.id == curId);
        Sun fnd = SpaceManager.suns.Find(f => f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.suns.Find(f => f.id == curId);
        }
        id = curId;
        return id;
    }
    public void MinimapDestroy()
    {
        base.Destroy();
    }
    public override void Destroy()
    {
        galaxyId = -1;
        systemId = -1;
        id = -1;

        base.Destroy();
    }
    public static void InvokeFixLightDir()
    {
        OnFixLightDirAction?.Invoke();
    }
}