using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;

public class SpaceController : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public UiSpaceObject uiSpaceObject;
    public Space space;
    public byte layer = 0;

    public bool isInitialized = false;
    public bool isDestroyed = false;

    public static UnityAction<byte> OnChangeLayer;

    void Start()
    {
        if (!isInitialized)
        {
            Init();
        }
    }

    void Update()
    {
        
    }

    public void Init()
    {
        OnChangeLayer += ChangeLayer;
        isInitialized = true;
    }

    public void DestroyObj()
    {
        if (!isDestroyed) {
            OnChangeLayer -= ChangeLayer;
            if (uiSpaceObject != null)
            {
                uiSpaceObject.DestroyObj();
            }
            if (gameObject != null)
            {
                DestroyImmediate(this.gameObject);
            }
            isDestroyed = true;
        }
    }

    public void ChangeLayer(byte layer)
    {
        if(this.layer != layer)
        {
            this.meshRenderer.enabled = false;
        }
        else
        {
            if (layer == 0)
            {
                Galaxy galaxy = SpaceManager.GetGalaxyByID(NetClient.localClient.galaxyId);
                if(MinimapPanel.selectedGalaxyId >= 0)
                {
                    galaxy = SpaceManager.GetGalaxyByID(MinimapPanel.selectedGalaxyId);
                }
                if (galaxy != null)
                {
                    CameraManager.minimapCamera.gameObject.SetActive(false);
                    CameraManager.minimapCamera.transform.localPosition = galaxy.GetPosition() + CameraController.startCamPositions[layer];
                    CameraManager.minimapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
                    CameraManager.minimapCamera.gameObject.SetActive(true);
                }
            }
            else if (layer == 1)
            {
                StarSystem system = SpaceManager.GetSystemByID(NetClient.localClient.galaxyId, NetClient.localClient.systemId);
                if (MinimapPanel.selectedSystemId >= 0)
                {
                    //system = SpaceManager.GetSystemByID(MinimapPanel.selectedGalaxyId, MinimapPanel.selectedSystemId);
                }

                if (system != null)
                {
                    CameraManager.minimapCamera.gameObject.SetActive(false);
                    CameraManager.minimapCamera.transform.localPosition = system.GetPosition() + CameraController.startCamPositions[layer];
                    CameraManager.minimapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
                    CameraManager.minimapCamera.gameObject.SetActive(true);
                }
            }
            else if (layer == 2)
            {
                Sector sp = SpaceManager.GetSectorByID(NetClient.localClient.galaxyId, NetClient.localClient.systemId, NetClient.localClient.sectorId);

                if (sp != null)
                {
                    CameraManager.minimapCamera.gameObject.SetActive(false);
                    CameraManager.minimapCamera.transform.position = sp.GetPosition() / Sector.minimapDivFactor + CameraController.startCamPositions[layer];
                    CameraManager.minimapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
                    CameraManager.minimapCamera.gameObject.SetActive(true);
                }
            }
            else if (layer == 3)
            {
                Zone sp = SpaceManager.GetZoneByID(NetClient.localClient.galaxyId, NetClient.localClient.systemId, NetClient.localClient.sectorId, NetClient.localClient.zoneId);
                if (MinimapPanel.selectedZoneId >= 0)
                {
                    //system = SpaceManager.GetSystemByID(MinimapPanel.selectedGalaxyId, MinimapPanel.selectedSystemId);
                }

                if (sp != null)
                {
                    CameraManager.minimapCamera.gameObject.SetActive(false);
                    CameraManager.minimapCamera.transform.position = sp.GetPosition() / Zone.minimapDivFactor + CameraController.startCamPositions[layer];
                    CameraManager.minimapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
                    CameraManager.minimapCamera.gameObject.SetActive(true);
                }
            }

            this.meshRenderer.enabled = true;
            MinimapPanel.layer = layer;
        }
    }


    public static void InvokeChangeLayer(byte layer)
    {
        OnChangeLayer?.Invoke(layer);
    }
}
