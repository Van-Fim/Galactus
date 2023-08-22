using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotController : Controller
{
    public void Start()
    {
        maxSpeed = 5000;
        rotationSpeed = 150;
        velocity = 10000;
    }
}