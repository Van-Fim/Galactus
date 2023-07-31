using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : CameraController
{
    public void SetPositionByLayer(int layer)
    {
        Vector3 addPos = Vector3.zero;
        if (layer == 0)
        {
            Galaxy space = MapSpaceManager.singleton.GetGalaxyByID(MapSpaceManager.selectedGalaxyId);
            if (space != null)
            {
                addPos = space.GetPosition();
            }
        }
        else if (layer == 1)
        {
            StarSystem space = MapSpaceManager.singleton.GetSystemByID(MapSpaceManager.selectedGalaxyId, MapSpaceManager.selectedSystemId);
            if (space != null)
            {
                addPos = space.GetPosition();
            }
        }
        Vector3 pos = CameraController.startCamPositions[layer];
        CameraManager.UpdateCamPos(CameraManager.minimapCamera, pos + addPos, new Vector3(90, 0, 0));
    }
}