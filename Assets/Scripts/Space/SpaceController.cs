using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;

public class SpaceController : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Space space;
    public byte layer = 0;

    public bool isInitialized = false;

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

    void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public void Init()
    {
        OnChangeLayer += ChangeLayer;
        isInitialized = true;
    }

    public void ChangeLayer(byte layer)
    {
        if (this.layer != layer)
        {
            this.meshRenderer.enabled = false;
        }
        else
        {
            if (layer == 0)
            {
                Galaxy galaxy = SpaceManager.GetGalaxyByID(MapClientPanel.selectedGalaxyId);

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
                StarSystem system = SpaceManager.GetSystemByID(MapClientPanel.selectedGalaxyId, MapClientPanel.selectedSystemId);

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
                Sector sp = SpaceManager.GetSectorByID(MapClientPanel.selectedGalaxyId, MapClientPanel.selectedSystemId, MapClientPanel.selectedSectorId);

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
                Zone sp = SpaceManager.GetZoneByID(MapClientPanel.selectedGalaxyId, MapClientPanel.selectedSystemId, MapClientPanel.selectedSectorId, MapClientPanel.selectedZoneId);

                if (sp != null)
                {
                    CameraManager.minimapCamera.gameObject.SetActive(false);
                    CameraManager.minimapCamera.transform.position = sp.GetPosition() / Zone.minimapDivFactor + CameraController.startCamPositions[layer];
                    CameraManager.minimapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
                    CameraManager.minimapCamera.gameObject.SetActive(true);
                }
            }

            MapClientPanel.currentLayer = layer;
        }
    }


    public static void InvokeChangeLayer(byte layer)
    {
        OnChangeLayer?.Invoke(layer);
    }
}
