using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpaceObjectNetManager : NetworkBehaviour
{
    public static SpaceObjectNetManager singleton;
    public SyncList<Pilot> pilots = new SyncList<Pilot>();
    public SyncList<Ship> ships= new SyncList<Ship>();
    public SyncList<Asteroid> asteroids= new SyncList<Asteroid>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public override void OnStartClient()
    {
        singleton = this;
    }
}
