using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCamera : MonoBehaviour
{
    public bool startUpdate;
    void Update()
    {
        if (startUpdate)
        {
            if (Client.localClient.isHyperMode)
            {
                transform.localPosition = (Client.localClient.currSector.GetPosition() + Client.localClient.currZone.GetPosition() +  CameraManager.mainCamera.transform.position)/SolarObject.hyperScaleFactor;
            }
            else{
                transform.localPosition = (Client.localClient.currSector.GetPosition() + Client.localClient.currZone.GetPosition() +  CameraManager.mainCamera.transform.position)/SolarObject.scaleFactor;
            }
            transform.localRotation = CameraManager.mainCamera.curCamera.transform.rotation;
        }
    }
}
