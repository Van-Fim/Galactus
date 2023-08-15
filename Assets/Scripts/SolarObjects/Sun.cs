using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class Sun : SolarObject
{
    public Light sunLight;
    public SunDirectionalLight directionalLight;
    public static UnityAction OnFixLightDirAction;
    public Sun(SpaceManager spm, StarSystem system, string templateName, int minRange = 0, int maxRange = 0)
    {
        spaceManager = spm;
        Color32 color = new Color32(system.color[0], system.color[1], system.color[2], system.color[3]);
        this.SetColor(color);
        this.galaxyId = system.galaxyId;
        this.systemId = system.id;
        Template sunTemplate = TemplateManager.FindTemplate(templateName, "sun");
        model = sunTemplate.GetValue("model", "patch");
        int scaleMin = int.Parse(sunTemplate.GetValue("scale", "min"));
        int scaleMax = int.Parse(sunTemplate.GetValue("scale", "max"));
        this.scale = Random.Range(scaleMin, scaleMax);

        int range = maxRange - minRange;
        float curDistance = 0;
        float sumDistance = 0;
        float dist2 = scale;
        int repeatCount = 30;
        Vector2 centerPosition = Vector3.zero;
        Vector2 sunPosition = Vector3.zero;
        bool found = true;
        List<Sun> allSystemSuns = spaceManager.suns.FindAll(f => f.galaxyId == system.galaxyId && f.id == system.id);
        while (repeatCount > 0 && found)
        {
            Vector2 vPosition = UnityEngine.Random.insideUnitCircle * (range);
            Vector2 fPosition = (vPosition.normalized * minRange);
            Vector2 pos = fPosition + vPosition;
            sunPosition = new Vector3(pos.x, 0, pos.y);
            allSystemSuns = spaceManager.suns.FindAll(f => f.galaxyId == system.galaxyId && f.id == system.id);
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
                sumDistance = (sn.scale + this.scale) * 3;
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
        spaceManager.suns.Add(this);
    }
    public override void Init()
    {
        OnFixLightDirAction += OnFixLightDir;
        base.Init();
    }
    public void OnFixLightDir()
    {
        if (directionalLight != null)
        {
            Sector sec = NetClient.singleton.Sector;
            Zone zn = NetClient.singleton.Zone;
            Vector3 sPos = sec.GetPosition() / SolarObject.scaleFactor;
            Vector3 zPos = zn.GetPosition() / SolarObject.scaleFactor;
            Vector3 cPos = CameraManager.mainCamera.transform.position / SolarObject.scaleFactor;
            Vector3 thPos = GetPosition();
            Vector2 pos1 = new Vector3(thPos.x, 0, thPos.z);
            Vector3 posFix = new Vector3(sPos.x, 0, sPos.z) + new Vector3(zPos.x, 0, zPos.z) + new Vector3(cPos.x, 0, cPos.z);
            Vector2 pos2 = new Vector2(posFix.x, posFix.z);
            Vector2 dir = (pos1 - pos2).normalized;
            directionalLight.transform.localEulerAngles = new Vector3(0, dir.y * 180, 0);
            DebugConsole.Log(dir);
            //Debug.DrawLine(position01, position02, Color.green);
        }
    }
    public override void OnRenderMinimap()
    {
        RenderAct();
    }
    public override void RenderAct()
    {
        if ((!(spaceManager is MapSpaceManager) && LocalClient.GetGalaxyId() == galaxyId && LocalClient.GetSystemId() == systemId) || (spaceManager is MapSpaceManager && MapSpaceManager.selectedGalaxyId == galaxyId && MapSpaceManager.selectedSystemId == systemId && MapClientPanel.currentLayer > 1))
        {
            if (main == null)
            {
                StarSystem sys = spaceManager.GetSystemByID(galaxyId, systemId);
                solarController = new GameObject().AddComponent<SolarController>();
                solarController.transform.SetParent(spaceManager.solarContainer.transform);
                solarController.transform.localPosition = GetPosition();
                solarController.transform.eulerAngles = GetRotation();
                solarController.solarObject = this;
                GameObject sunGameobject = Resources.Load<GameObject>($"{model}/MAIN");
                main = GameObject.Instantiate(sunGameobject, solarController.transform);
                float fscale = scale;
                solarController.gameObject.layer = 7;
                GameObject hull = main.transform.Find("HULL").gameObject;
                sunLight = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<Light>("SunLightPrefab"));

                if (spaceManager is MapSpaceManager)
                {
                    spaceManager.solarContainer.gameObject.layer = 6;
                    solarController.gameObject.layer = 6;
                    main.layer = 6;
                    hull.gameObject.layer = 6;
                    sunLight.gameObject.layer = 6;
                }

                main.transform.localScale = new Vector3(fscale, fscale, fscale);

                Color32 col = sys.GetColor();

                hull.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", col);
                hull.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", col);
                hull.GetComponent<MeshRenderer>().material.SetColor("_TintColor", col);


                sunLight.transform.SetParent(main.transform);
                sunLight.transform.localPosition = Vector3.zero;
                sunLight.color = col;

                if (spaceManager is not MapSpaceManager)
                {
                    directionalLight = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<SunDirectionalLight>("SunDirectionalLightPrefab"));
                    directionalLight.Color = col;
                }

                solarController.gameObject.name = "Sun_" + id.ToString();

            }
        }
        else
        {
            MinimapDestroy();
        }
    }

    public int GenerateId()
    {
        int curId = 0;
        StarSystem sys = spaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == systemId && f.id == curId);
        Sun fnd = spaceManager.suns.Find(f => f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = spaceManager.suns.Find(f => f.id == curId);
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
        OnRenderAction -= OnRender;
        OnFixLightDirAction -= OnFixLightDir;
        if (spaceManager is MapSpaceManager)
        {
            OnRenderMinimapAction -= OnRenderMinimap;
        }
        if (directionalLight)
        {
            GameObject.DestroyImmediate(directionalLight.gameObject);
        }
        base.Destroy();
    }
    public static void InvokeFixLightDir()
    {
        OnFixLightDirAction?.Invoke();
    }
}