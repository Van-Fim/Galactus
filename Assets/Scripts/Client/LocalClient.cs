using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalClient
{
    public static Vector3 GetObjectPosition()
    {
        return NetClient.singleton.GetObjectPosition();
    }
    public static int GetGalaxyId()
    {
        return NetClient.singleton.GetGalaxyId();
    }
    public static int GetSystemId()
    {
        return NetClient.singleton.GetSystemId();
    }
    public static int GetSectorId()
    {
        return NetClient.singleton.GetSectorId();
    }
    public static int GetZoneId()
    {
        return NetClient.singleton.GetSectorId();
    }
    public static Vector3 GetSectorIndexes()
    {
        return NetClient.singleton.GetSectorIndexes();
    }
    public static Vector3 GetZoneIndexes()
    {
        return NetClient.singleton.GetZoneIndexes();
    }
    public static void SetSectorIndexes(Vector3 value, bool characterToo = false)
    {
        NetClient.singleton.SetSectorIndexes(value, characterToo);
    }
    public static void SetZoneIndexes(Vector3 value, bool characterToo = false)
    {
        NetClient.singleton.SetZoneIndexes(value, characterToo);
    }
    public static Galaxy GetGalaxy()
    {
        Galaxy ret = null;
        ret = SpaceManager.singleton.GetGalaxyByID(GetGalaxyId());
        return ret;
    }
    public static StarSystem GetSystem()
    {
        StarSystem ret = null;
        ret = SpaceManager.singleton.GetSystemByID(GetGalaxyId(), GetSystemId());
        return ret;
    }
    public static Sector GetSector()
    {
        Sector ret = null;
        ret = SpaceManager.singleton.GetSectorByID(GetGalaxyId(), GetSystemId(), GetSectorId());
        return ret;
    }
    public static Zone GetZone()
    {
        Zone ret = null;
        ret = SpaceManager.singleton.GetZoneByID(GetGalaxyId(), GetSystemId(), GetSectorId(), GetZoneId());
        return ret;
    }
    public static bool GetIsGameStartDataLoaded()
    {
        return NetClient.singleton.GetIsGameStartDataLoaded();
    }
    public static void SetIsGameStartDataLoaded(bool value)
    {
        NetClient.singleton.SetIsGameStartDataLoaded(value);
    }
    public static string GetGamestartTemplateName()
    {
        string ret = null;
        ret = NetClient.singleton.characterData.gameStart;
        return ret;
    }
    public static string GetCharacterLogin()
    {
        string ret = null;
        ret = NetClient.singleton.characterData.login;
        return ret;
    }
    public static void SetCharacterLogin(string login)
    {
        NetClient.singleton.characterData.login = login;
    }
    public static SpaceObject GetControlledObject()
    {
        SpaceObject ret = NetClient.singleton.ControlledObject;
        return ret;
    }
    public static void SetControlledObject(SpaceObject spaceObject)
    {
        NetClient.singleton.ControlledObject = spaceObject;
        NetClient.singleton.ControlledObject.isPlayerControll = true;
    }
}
