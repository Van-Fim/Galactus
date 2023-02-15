using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCamera : MonoBehaviour
{
    public bool startUpdate;
    public Camera curCamera;
    public static Vector3 fixPos;
    void Update()
    {
        if (startUpdate)
        {
            if (Client.localClient.isHyperMode)
            {
                transform.localPosition = (Client.localClient.currSector.GetPosition() + Client.localClient.currZone.GetPosition() +  Client.localClient.controllTarget.transform.localPosition + CameraManager.mainCamera.transform.localPosition)/SolarObject.hyperScaleFactor - CameraManager.mainCamera.transform.localPosition;
            }
            else{
                transform.localPosition = (Client.localClient.currSector.GetPosition() + Client.localClient.currZone.GetPosition() +  Client.localClient.controllTarget.transform.localPosition + CameraManager.mainCamera.transform.localPosition)/SolarObject.scaleFactor;
            }
            transform.localRotation = CameraManager.mainCamera.curCamera.transform.rotation;
        }
    }
}
