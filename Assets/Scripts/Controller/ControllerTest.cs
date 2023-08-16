using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTest : MonoBehaviour
{
    public Light dirLight;
    Rigidbody rigidbodyMain;
    float val = 0;
    float val2 = 0;

    public int maxSpeed = 1000;
    public int rotationSpeed = 150;
    public int velocity = 10000;
    public bool isHyperMode = false;

    public static bool blocked = false;
    void Awake()
    {
        rigidbodyMain = gameObject.GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (rigidbodyMain == null)
        {
            return;
        }
        Turn();
        Move();
    }

    void Update()
    {
        Vector3 playerPos = transform.localPosition;
        Vector3 sunPos = new Vector3(1800, 0, 0);

        Vector2 pos1 = new Vector3(sunPos.x, 0, sunPos.z);
        Vector2 pos2 = new Vector3(playerPos.x, 0, playerPos.z);
        Vector2 dir = (pos2 - pos1).normalized;

        // Ерунда получается 
        dirLight.transform.localEulerAngles = new Vector3(0, dir.y * 180, 0);
 
        dirLight.transform.LookAt(transform.localPosition);
    }

    public virtual void Turn()
    {
        if (!Input.GetMouseButton(1))
        {
            float ControlHorizontal = Input.GetAxis("Horizontal");
            float ControlVertical = Input.GetAxis("Vertical");

            rigidbodyMain.transform.Rotate(Vector3.up * ControlHorizontal * 5);
            rigidbodyMain.transform.Rotate(Vector3.right * ControlVertical * 5);
        }
        else
        {
            float speed = rotationSpeed;
            float iroll = Input.GetAxis("Roll");
            float yaw = 0;
            float pitch = 0;
            float roll = 0;
            float DeadZone = 0.08f;
            Vector3 mousePos = Input.mousePosition;
            roll = speed * Time.deltaTime * iroll;

            pitch = (mousePos.y - (Screen.height * 0.5f)) / (Screen.height * 0.5f);
            yaw = (mousePos.x - (Screen.width * 0.5f)) / (Screen.width * 0.5f);
            pitch *= 3;
            yaw *= 3;


            // Make sure the values don't exceed limits.
            pitch = -Mathf.Clamp(pitch, -1.0f, 1.0f);
            yaw = Mathf.Clamp(yaw, -1.0f, 1.0f);
            if (pitch <= DeadZone && pitch > 0)
                pitch = 0;
            if (pitch >= -DeadZone && pitch < 0)
                pitch = 0;
            if (yaw <= DeadZone && yaw > 0)
                yaw = 0;
            if (yaw >= -DeadZone && yaw < 0)
                yaw = 0;

            float ayaw = (yaw - DeadZone);

            Vector3 ang = new Vector3(pitch * (speed / 100), yaw * (speed / 100), roll);
            rigidbodyMain.transform.Rotate(ang);
        }
    }

    public virtual void Move()
    {
        if (Input.GetKey("space") && rigidbodyMain != null)
        {
            rigidbodyMain.velocity = Vector3.zero;
            val2 = val = 0;
            return;
        }
        float changeFactor = Input.GetAxis("ChangeSpeed");

        val += changeFactor;

        if (val > 1)
        {
            val = 1;
        }
        if (val < -0.25f)
        {
            val = -0.25f;
        }

        float speed1 = velocity;

        if (val2 < val)
        {
            val2 += speed1;
            if (val2 > val)
            {
                val2 = val;
            }
        }
        else if (val2 > val)
        {
            val2 -= speed1;
            if (val2 < val)
            {
                val2 = val;
            }
        }

        rigidbodyMain.AddForce(transform.forward * maxSpeed * val2);
        string zn = "";
        if (val2 < 0)
        {
            zn = "-";
        }
    }
}