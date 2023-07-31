using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sun : SolarObject
{
    public Light sunLight;
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

        while (repeatCount > 0 && found)
        {
            Vector2 vPosition = UnityEngine.Random.insideUnitCircle * (range);
            Vector2 fPosition = (vPosition.normalized * minRange);
            Vector2 pos = fPosition + vPosition;
            sunPosition = new Vector3(pos.x, 0, pos.y);
            if (spaceManager.suns.Count == 0)
            {
                found = false;
                break;
            }
            for (int i = 0; i < spaceManager.suns.Count; i++)
            {
                Sun sn = spaceManager.suns[i];
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
        Sun fsun = spaceManager.suns.Find(f => f.id == findId);
        while (fsun != null)
        {
            findId++;
            fsun = spaceManager.suns.Find(f => f.id == findId);
        }
        this.id = findId;
        spaceManager.suns.Add(this);
    }

    public override void OnRenderMinimap()
    {
        RenderAct();
    }
    public override void RenderAct()
    {
        if ((!(spaceManager is MapSpaceManager) && NetClient.GetGalaxyId() == galaxyId && NetClient.GetSystemId() == systemId) || (spaceManager is MapSpaceManager && MapSpaceManager.selectedGalaxyId == galaxyId && MapSpaceManager.selectedSystemId == systemId && MapClientPanel.currentLayer > 1))
        {
            if (main == null)
            {
                StarSystem sys = spaceManager.GetSystemByID(galaxyId, systemId);
                solarController = new GameObject().AddComponent<SolarController>();
                solarController.transform.SetParent(spaceManager.solarContainer.transform);
                solarController.transform.localPosition = GetPosition();
                solarController.transform.eulerAngles = GetRotation();
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
        if (spaceManager is MapSpaceManager)
        {
            OnRenderMinimapAction -= OnRenderMinimap;
        }
        base.Destroy();
    }
}