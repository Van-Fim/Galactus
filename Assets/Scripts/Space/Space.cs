using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
namespace GameContent
{
    [Serializable]
    public abstract class Space
    {
        public int id = -1;
        public int size = 10;
        public string templateName;
        public string name;
        public SpaceMapObject spaceMapObject;
        public int[] indexes = { 0, 0, 0 };
        public float[] position = new float[] { 0, 0, 0 };
        public float[] rotation = new float[] { 0, 0, 0 };
        public byte[] color = new byte[] { 255, 255, 255, 255 };
        public byte[] bgcolor = new byte[] { 255, 255, 255, 255 };
        public SpaceManager spaceManager;
        public SpaceUiObj spaceUiObj;
        public static UnityAction OnRenderAction;
        public static UnityAction OnMinimapRenderAction;
        public static UnityAction OnDrawUiAction;
        public static UnityAction OnClearAllControllersAction;

        public Space() { }
        public Space(string templateName)
        {
            this.templateName = templateName;
        }
        public virtual void Init()
        {
            OnRenderAction += OnRender;
            OnMinimapRenderAction += OnMinimapRender;
            OnDrawUiAction += OnDrawUi;
            OnClearAllControllersAction += OnClearAllControllers;
        }
        public virtual void Destroy()
        {
            id = -1;
            OnRenderAction -= OnRender;
            OnRenderAction -= OnMinimapRender;
            OnDrawUiAction -= OnDrawUi;
            OnClearAllControllersAction -= OnClearAllControllers;
            templateName = null;
        }
        public virtual void OnMinimapRender()
        {
        }
        public virtual void OnRender()
        {
            DestroyController(0);
            DestroyController(1);
            DestroyController(2);
            DestroyController(3);
            if (this is (Galaxy))
            {
                Vector3 pos = GetPosition();
                Vector3 rot = GetRotation();
                SpaceMapObject spmPrefab = GamePrefabsManager.singleton.LoadPrefab<SpaceMapObject>("GalaxyMapObjectPrefab");
                if (MapClientPanel.currentLayer == spmPrefab.layer)
                {
                    if (spaceMapObject == null)
                    {
                        spaceMapObject = GameObject.Instantiate(spmPrefab, SpaceManager.singleton.transform);
                        spaceMapObject.layer = spmPrefab.layer;
                        spaceMapObject.transform.localPosition = pos;
                        spaceMapObject.transform.localEulerAngles = rot;
                        spaceMapObject.meshRenderer.material.SetColor("_TintColor", GetColor());
                        spaceMapObject.meshRenderer.material.SetColor("_Color", GetColor());
                        spaceMapObject.meshRenderer.enabled = true;
                        spaceMapObject.Init();

                        //DebugConsole.Log($"id rendered {id}");
                    }
                }
                else
                {
                    //DestroyController(spmPrefab.layer);
                }
            }
            else if (this is (StarSystem))
            {
                StarSystem val = (StarSystem)this;
                if (val.galaxyId != MapSpaceManager.selectedGalaxyId)
                {
                    return;
                }
                Vector3 pos = GetPosition();
                Vector3 rot = GetRotation();
                SpaceMapObject spmPrefab = GamePrefabsManager.singleton.LoadPrefab<SpaceMapObject>("SystemMapObjectPrefab");
                if (MapClientPanel.currentLayer == spmPrefab.layer)
                {
                    if (spaceMapObject == null)
                    {
                        spaceMapObject = GameObject.Instantiate(spmPrefab, SpaceManager.singleton.transform);
                        spaceMapObject.layer = spmPrefab.layer;
                        spaceMapObject.transform.localPosition = pos;
                        spaceMapObject.transform.localEulerAngles = rot;
                        spaceMapObject.meshRenderer.material.SetColor("_TintColor", GetColor());
                        spaceMapObject.meshRenderer.material.SetColor("_Color", GetColor());
                        spaceMapObject.meshRenderer.enabled = true;
                        spaceMapObject.Init();

                        //DebugConsole.Log($"id rendered {id}");
                    }
                }
                else
                {
                    //DestroyController(spmPrefab.layer);
                }
            }
            else if (this is (Sector) && MapClientPanel.currentLayer == 2)
            {
                Sector val = (Sector)this;
                if (val.galaxyId != MapSpaceManager.selectedGalaxyId || val.systemId != MapSpaceManager.selectedSystemId)
                {
                    return;
                }
                Vector3 pos = GetPosition();
                Vector3 rot = GetRotation();
                SpaceMapObject spmPrefab = GamePrefabsManager.singleton.LoadPrefab<SpaceMapObject>("SectorMapObjectPrefab");
                if (MapClientPanel.currentLayer == spmPrefab.layer)
                {
                    if (spaceMapObject == null)
                    {
                        spaceMapObject = GameObject.Instantiate(spmPrefab, SpaceManager.singleton.transform);
                        spaceMapObject.layer = spmPrefab.layer;
                        spaceMapObject.transform.localPosition = pos;
                        spaceMapObject.transform.localEulerAngles = rot;
                        spaceMapObject.meshRenderer.material.SetColor("_TintColor", GetColor());
                        spaceMapObject.meshRenderer.material.SetColor("_Color", GetColor());
                        spaceMapObject.meshRenderer.enabled = true;
                        spaceMapObject.Init();
                    }
                }
                else
                {
                    //DestroyController(spmPrefab.layer);
                }
            }
            else if (this is (Zone) && MapSpaceManager.selectedZoneId == id && MapClientPanel.currentLayer == 3)
            {
                Zone val = (Zone)this;
                DebugConsole.Log($"Galaxy {val.galaxyId} System {val.systemId} Sector {val.sectorId} Zone {id}");
            }
        }
        public virtual void DestroyController(int layer)
        {
            if (spaceMapObject != null && spaceMapObject.layer == layer)
            {
                GameObject.DestroyImmediate(spaceMapObject.gameObject);
            }
        }
        public virtual void OnClearAllControllers()
        {
        }
        public virtual void OnDrawUi()
        {
            if (spaceUiObj == null && spaceMapObject != null)
            {
                spaceUiObj = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<SpaceUiObj>("SpaceUiObj"), CanvasManager.canvas.transform);
                spaceUiObj.space = this;
                spaceUiObj.transform.localPosition = Vector3.zero;
                spaceUiObj.transform.localRotation = Quaternion.identity;
                spaceUiObj.Init();
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
        public void SetIndexes(Vector3 indexes)
        {
            this.indexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
        }
        public Vector3 GetIndexes()
        {
            return new Vector3((int)this.indexes[0], (int)this.indexes[1], (int)this.indexes[2]);
        }
        public static Vector3 RecalcPos(Vector3 position, float stepValue, bool recl = false)
        {
            Vector3 ret = new Vector3();
            Vector3 c1 = position;
            Vector3 stepVals = new Vector3(stepValue, stepValue, stepValue);
            position = new Vector3(position.x + stepVals.x / 2, position.y + stepVals.y / 2, position.z + stepVals.z / 2);
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
            ret = ret * stepValue;
            return ret;
        }
        public static void InvokeClearAllControllers()
        {
            OnClearAllControllersAction?.Invoke();
        }
        public static void InvokeRender()
        {
            OnRenderAction?.Invoke();
        }
        public static void InvokeMinimapRender()
        {
            OnMinimapRenderAction?.Invoke();
        }
        public static void InvokeDrawUi()
        {
            OnDrawUiAction?.Invoke();
        }
    }
}