using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public void Start()
    {
        maxSpeed = 1000;
        rotationSpeed = 150;
        velocity = 10000;
    }
}
