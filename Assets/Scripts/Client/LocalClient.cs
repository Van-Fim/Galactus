using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public static class LocalClient
{
    public static uint GetNetId()
    {
        return NetClient.singleton.netId;
    }
    public static CharacterData GetCharacterData()
    {
        return NetClient.singleton.characterData;
    }
    public static Vector3 GetObjectPosition()
    {
        return NetClient.singleton.GetObjectPosition();
    }
    public static Vector3 GetPosition()
    {
        return NetClient.singleton.characterData.GetPosition();
    }
    public static Vector3 GetRotation()
    {
        return NetClient.singleton.characterData.GetRotation();
    }
    public static void SetPosition(Vector3 position)
    {
        NetClient.singleton.characterData.SetPosition(position);
    }
    public static void SetRotation(Vector3 rotation)
    {
        NetClient.singleton.characterData.SetRotation(rotation);
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
        return NetClient.singleton.GetZoneId();
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
    public static void FixSpace()
    {
        NetClient.singleton.FixSpace();
    }
    public static void FixPos()
    {
        NetClient.singleton.FixPos();
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
