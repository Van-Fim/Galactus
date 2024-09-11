using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Mirror;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SpaceObject : MonoBehaviour
{
    public uint id;

    public string objectName;
    public GameObject main;
    public GameObject hull;
    public SpaceManager spaceManager;
    public string templateName;
    public int galaxyId;
    public int systemId;
    public int sectorId;
    public string hardpointsTemplateName;
    public string modelPatch;

    public SOController controller;
    public IND_target iND_Target;

    public int[] sectorIndexes = { 0, 0, 0 };
    public int[] zoneIndexes = { 0, 0, 0 };

    Galaxy currGalaxy;
    SpaceSystem currSystem;
    Sector currSector;

    public List<Hardpoint> hardpoints = new List<Hardpoint>();

    public Rigidbody rigidbodyMain;
    public int mass;
    public float drag;
    public float angulardrag;

    public float scaleFactor = 1f;

    public bool isPlayerControll;
    public bool isInitialized;

    public static UnityAction OnRenderAction;
    public static UnityAction OnRenderINDAction;
    public SpaceObject() { }

    public virtual void Init()
    {
        objectName = $"{GetType()}_{id}";
        OnRenderAction += OnRender;
        OnRenderINDAction += OnRenderInd;
    }
    public virtual void Destroy()
    {
        OnRenderAction -= OnRender;
        OnRenderINDAction -= OnRenderInd;
        isInitialized = false;
        if (iND_Target)
        {
            IND_targetManager.Remove(iND_Target);
        }
        if (controller != null)
        {
            GameObject.DestroyImmediate(controller);
        }
        if (rigidbodyMain != null)
        {
            GameObject.DestroyImmediate(rigidbodyMain);
        }
        GameObject.DestroyImmediate(this);
    }
    public void ReadSpaceObjectData(SpaceObjectData spaceObjectData)
    {
        id = spaceObjectData.id;
        templateName = spaceObjectData.templateName;
        hardpointsTemplateName = spaceObjectData.hardpointsTemplateName;
        galaxyId = spaceObjectData.galaxyId;
        systemId = spaceObjectData.systemId;
        sectorId = spaceObjectData.sectorId;
        isPlayerControll = spaceObjectData.isPlayerControll;
        sectorIndexes = spaceObjectData.sectorIndexes;
        zoneIndexes = spaceObjectData.zoneIndexes;
        modelPatch = spaceObjectData.modelPatch;
        mass = spaceObjectData.mass;
        drag = spaceObjectData.drag;
        angulardrag = spaceObjectData.angulardrag;
        transform.localPosition = spaceObjectData.GetPosition();
        transform.localEulerAngles = spaceObjectData.GetRotation();
    }
    public SpaceObjectData GetSpaceObjectData()
    {
        SpaceObjectData ret = new SpaceObjectData();
        ret.id = id;
        ret.spaceObjectId = id;
        ret.templateName = templateName;
        ret.hardpointsTemplateName = hardpointsTemplateName;
        ret.galaxyId = galaxyId;
        ret.systemId = systemId;
        ret.type = GetObjectType();
        ret.isPlayerControll = isPlayerControll;
        ret.modelPatch = modelPatch;
        ret.mass = mass;
        ret.drag = drag;
        ret.angulardrag = angulardrag;
        ret.SetPosition(transform.localPosition);
        ret.SetRotation(transform.localEulerAngles);
        ret.SetSectorIndexes(GetSectorIndexes());
        ret.SetZoneIndexes(GetZoneIndexes());
        return ret;
    }
    public Vector3 GetZoneIndexes()
    {
        return new Vector3((int)this.zoneIndexes[0], (int)this.zoneIndexes[1], (int)this.zoneIndexes[2]);
    }
    public Vector3 GetSectorIndexes()
    {
        return new Vector3((int)this.sectorIndexes[0], (int)this.sectorIndexes[1], (int)this.sectorIndexes[2]);
    }
    public void SetZoneIndexes(Vector3 value)
    {
        zoneIndexes = new int[] { (int)value.x, (int)value.y, (int)value.z };
    }
    public void SetSectorIndexes(Vector3 value)
    {
        sectorIndexes = new int[] { (int)value.x, (int)value.y, (int)value.z };
    }
    public static SpaceObject Create(SpaceObjectData spaceObjectData, GameObject gmobj = null)
    {
        SpaceObject ret = null;
        NetSpaceObject netRet = null;
        if (gmobj == null)
        {
            ret = spaceObjectData.CreateByType();
        }
        else
        {
            ret = spaceObjectData.AddByType(gmobj);
        }
        netRet = ret.gameObject.GetComponent<NetSpaceObject>();
        ret.templateName = spaceObjectData.templateName;
        ret.galaxyId = spaceObjectData.galaxyId;
        ret.systemId = spaceObjectData.systemId;
        ret.sectorId = spaceObjectData.sectorId;
        ret.isPlayerControll = spaceObjectData.isPlayerControll;
        ret.isInitialized = spaceObjectData.isInitialized;
        ret.hardpointsTemplateName = spaceObjectData.hardpointsTemplateName;
        ret.mass = spaceObjectData.mass;
        ret.drag = spaceObjectData.drag;
        ret.scaleFactor = spaceObjectData.scaleFactor;
        if (ret.scaleFactor == 0)
        {
            ret.scaleFactor = 1;
        }
        ret.angulardrag = spaceObjectData.angulardrag;
        ret.modelPatch = spaceObjectData.modelPatch;
        ret.transform.localPosition = SpaceManager.spaceContainer.transform.localPosition + spaceObjectData.GetPosition();
        ret.transform.localEulerAngles = spaceObjectData.GetRotation();
        if (!ret.isPlayerControll && LocalClient.isServer)
        {
            if (netRet != null)
            {
                netRet.data = spaceObjectData;
            }
            NetworkServer.Spawn(ret.gameObject);
        }
        SpaceObjectManager.spaceObjects.Add(ret);
        return ret;
    }
    public virtual void Warp(SpaceSystem spaceSystem, int sectorId, Vector3 position, Vector3 rotation)
    {
        WarpSystem(spaceSystem, sectorId);
        SpaceManager.spaceContainer.transform.localPosition = Vector3.zero;

        if (rigidbodyMain != null)
        {
            rigidbodyMain.angularVelocity = Vector3.zero;
            rigidbodyMain.velocity = Vector3.zero;
        }
        transform.localPosition = position;
        transform.localEulerAngles = rotation;
    }
    public virtual void WarpSystem(SpaceSystem spaceSystem, int sectorId)
    {
        galaxyId = spaceSystem.galaxyId;
        systemId = spaceSystem.id;
        Sector sector = SpaceManager.sectors.Find(x => x.id == sectorId && x.galaxyId == galaxyId && x.systemId == systemId);
        Vector3 sPos = sector.GetPosition() + transform.localPosition;
        Vector3 sectorIndexes = new Vector3((int)(sPos.x / (PositionFixer.stepSize * 2)), (int)(sPos.y / (PositionFixer.stepSize * 2)), (int)(sPos.z / (PositionFixer.stepSize * 2)));
        SetSectorIndexes(sectorIndexes);
    }

    public virtual void LoadHardpoints()
    {
        if (hardpointsTemplateName.Length == 0 || hardpointsTemplateName == "0")
        {
            return;
        }
        Template temp = TemplateManager.FindTemplate(hardpointsTemplateName, "hardpoints");
        if (temp == null)
        {
            Debug.LogError($"Hardpoint template {hardpointsTemplateName} does not exist.");
            return;
        }
        List<TemplateNode> nodes = temp.GetNodeList("hardpoint");
        for (int i = 0; i < nodes.Count; i++)
        {
            TemplateNode node = nodes[i];
            int id = int.Parse(node.GetValue("id"));
            string type = node.GetValue("type");
            TemplateNode positionNode = node.GetChildNode("position");
            TemplateNode rotationNode = node.GetChildNode("rotation");
            Vector3 position = Vector3.zero;
            Vector3 rotation = Vector3.zero;
            if (position != null)
            {
                float x = XMLF.FloatVal(positionNode.GetValue("x"));
                float y = XMLF.FloatVal(positionNode.GetValue("y"));
                float z = XMLF.FloatVal(positionNode.GetValue("z"));
                position = new Vector3(x, y, z);
            }
            if (rotation != null)
            {
                float x = XMLF.FloatVal(rotationNode.GetValue("x"));
                float y = XMLF.FloatVal(rotationNode.GetValue("y"));
                float z = XMLF.FloatVal(rotationNode.GetValue("z"));
                rotation = new Vector3(x, y, z);
            }
            Hardpoint hardpoint = new Hardpoint();
            hardpoint.id = id;
            hardpoint.type = type;
            hardpoint.SetPosition(position);
            hardpoint.SetRotation(rotation);
            hardpoints.Add(hardpoint);
        }
    }
    public virtual Hardpoint GetHardpointByType(string type)
    {
        Hardpoint retHp = hardpoints.Find(f => f.type == type);
        return retHp;
    }
    public string GetObjectType()
    {
        return $"{GetType()}".ToLower();
    }
    public virtual void OnRenderInd()
    {
        if ((galaxyId != LocalClient.galaxyId || systemId != LocalClient.systemId) || (Hud.activeHud != null && Hud.activeHud.isHideInds))
        {
            if (iND_Target)
            {
                iND_Target.scaleFactor = 1;
                iND_Target.enabled = false;
                iND_Target.gameObject.SetActive(false);
                if ((galaxyId != LocalClient.galaxyId || systemId != LocalClient.systemId) && IND_target.selectedTarget == iND_Target)
                {
                    IND_target.selectedTarget = null;
                }
            }

            return;
        }
        if (!iND_Target && LocalClient.ControlledObject != this)
        {
            iND_Target = IND_targetManager.Create(this);
            iND_Target.Init();
        }
        if (iND_Target)
        {
            float thing = Vector3.Dot((transform.position - LocalClient.ControlledObject.transform.position).normalized, LocalClient.ControlledObject.transform.forward);
            float dist = Vector3.Distance(LocalClient.ControlledObject.transform.position, transform.position);
            bool b1 = thing <= 0, b2 = dist > 500000;
            if (IND_target.selectedTarget == iND_Target)
            {
                iND_Target.SetColor(IND_target.selectedColor);
                SOController.distanceToTarget = Math.Round(dist / 1000f, 2);
                b2 = false;
            }
            else
            {
                iND_Target.SetColor(IND_target.defColor);
            }
            if (b1 || b2)
            {
                iND_Target.enabled = false;
                iND_Target.gameObject.SetActive(false);
                return;
            }
            else
            {
                iND_Target.scaleFactor = 1 + 1 / (dist / 50000);
                iND_Target.enabled = true;
                iND_Target.gameObject.SetActive(true);
                iND_Target.FixObject();
            }
        }
    }
    public virtual void OnRender()
    {
        if (!isPlayerControll)
        {
            transform.SetParent(SpaceManager.spaceContainer.transform);
        }
        if (modelPatch.Length > 0 && !main)
        {
            GameObject minst = Resources.Load<GameObject>($"{modelPatch}/MAIN");
            main = Instantiate(minst, gameObject.transform);
            main.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }

        if (galaxyId != LocalClient.galaxyId || systemId != LocalClient.systemId)
        {
            if (main != null)
            {
                main.SetActive(false);
                //Destroy(rigidbodyMain);
            }
        }
        else
        {
            if (main != null)
            {
                main.SetActive(true);
            }
        }
        if (main != null && rigidbodyMain == null)
        {
            rigidbodyMain = this.gameObject.GetComponent<Rigidbody>();
            rigidbodyMain.mass = mass;
            rigidbodyMain.drag = drag;
            rigidbodyMain.angularDrag = angulardrag;
            rigidbodyMain.useGravity = false;

            hull = main.transform.Find("HULL").gameObject;
            // if (!isPlayerControll)
            // {
            //     NetworkTransformUnreliable networkTransform = this.gameObject.GetComponent<NetworkTransformUnreliable>();
            //     networkTransform.syncDirection = SyncDirection.ServerToClient;
            //     NetworkRigidbodyUnreliable netRigidbodyMain = this.gameObject.GetComponent<NetworkRigidbodyUnreliable>();
            //     //netRigidbodyMain.syncDirection = SyncDirection.ServerToClient;
            // }
        }
    }
    public SpaceObject(int galaxyId, int systemId, int sectorId, string templateName)
    {
        this.galaxyId = galaxyId;
        this.systemId = systemId;
        this.sectorId = sectorId;
        this.templateName = templateName;
    }
    public static uint GetId()
    {
        uint id = 0;
        while (SpaceObjectManager.spaceObjects.Find(f => f.id == id) != null)
        {
            id++;
        }

        return id;
    }
    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }
    public static void InvokeIndRender()
    {
        OnRenderINDAction?.Invoke();
    }
}
