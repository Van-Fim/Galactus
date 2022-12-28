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

    public static UnityAction<byte> OnChangeLayer;

    void Start()
    {
        OnChangeLayer += ChangeLayer;
    }

    void Update()
    {
        
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
            
            this.meshRenderer.enabled = true;
            MinimapPanel.layer = layer;
        }
    }


    public static void InvokeChangeLayer(byte layer)
    {
        OnChangeLayer?.Invoke(layer);
    }
}
