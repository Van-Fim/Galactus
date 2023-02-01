using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPlayerController : Controller
{
    public void Start()
    {
        maxSpeed = 1000000;
        rotationSpeed = 150;
        velocity = 10000000;
    }
}
