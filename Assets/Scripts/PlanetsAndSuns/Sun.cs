using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sun : SolarObject
{
    public Light sunLight;
    public Sun(StarSystem system, string templateName, int minRange = 0, int maxRange = 0)
    {
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
            Vector2 vPosition = UnityEngine.Random.insideUnitCircle * range;
            Vector2 fPosition = (vPosition.normalized * minRange);
            Vector2 pos = fPosition + vPosition;
            sunPosition = new Vector3(pos.x, 0, pos.y) * SolarObject.scaleFactor;
            if (SpaceManager.suns.Count == 0)
            {
                found = false;
                break;
            }
            for (int i = 0; i < SpaceManager.suns.Count; i++)
            {
                Sun sn = SpaceManager.suns[i];
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
        Sun fsun = SpaceManager.suns.Find(f => f.id == findId);
        while (fsun != null)
        {
            findId++;
            fsun = SpaceManager.suns.Find(f => f.id == findId);
        }
        this.id = findId;
        SpaceManager.suns.Add(this);
    }

    public override void OnRender()
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
                GameObject hull = main.transform.Find("HULL").gameObject;
                Color32 col = sys.GetColor();

                hull.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", col);
                hull.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", col);
                
                sunLight = new GameObject().AddComponent<Light>();
                sunLight.transform.SetParent(SpaceManager.spaceContainer.transform);
                sunLight.transform.localPosition = GetPosition();
                sunLight.range = 1000000000;
                sunLight.color = col;
            }
        }
        else
        {
            Destroy();
        }
    }

    public int GenerateId()
    {
        int curId = 0;
        StarSystem sys = SpaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == systemId && f.id == curId);
        Sun fnd = SpaceManager.suns.Find(f => f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.suns.Find(f => f.id == curId);
        }
        id = curId;
        return id;
    }
}