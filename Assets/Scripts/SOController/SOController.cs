using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SOController : MonoBehaviour
{
    public SpaceObject obj;
    public static int mulVal = 1;
    private float val = 0;
    private float val2 = 0;

    private int testSectorId = 0;
    private bool testWarping = false;

    private long maxSpeed = 300;
    private int rotationSpeed = 150;
    private long velocity = 100;
    private bool isHyperMode = false;

    public static bool blocked = false;
    public static int currentSpeed = 0;
    public static double distanceToTarget = 0;
    public static int currentMaxSpeed = 0;

    void Awake()
    {
        obj = gameObject.GetComponent<SpaceObject>();
    }
    public void Start()
    {
        Template template = TemplateManager.FindTemplate(obj.templateName, obj.GetObjectType());
        TemplateNode paramsNode = template.GetNode("params");
        if (paramsNode != null)
        {
            this.maxSpeed = long.Parse(paramsNode.GetValue("maxspeed")) * mulVal;
            this.rotationSpeed = int.Parse(paramsNode.GetValue("rotationspeed"));
            this.velocity = long.Parse(paramsNode.GetValue("velocity")) * mulVal;
        }
    }
    void FixedUpdate()
    {
        if (obj.rigidbodyMain == null)
        {
            return;
        }
        if (obj.isPlayerControll && !blocked)
        {
            Turn();
            Move();
        }
    }

    void Update()
    {
        Template template = TemplateManager.FindTemplate(obj.templateName, obj.GetObjectType());
        TemplateNode paramsNode = template.GetNode("params");
        if (paramsNode != null)
        {
            this.maxSpeed = long.Parse(paramsNode.GetValue("maxspeed")) * mulVal;
            this.rotationSpeed = int.Parse(paramsNode.GetValue("rotationspeed"));
            this.velocity = long.Parse(paramsNode.GetValue("velocity")) * mulVal;
        }
    }

    public virtual void Turn()
    {
        if (!Input.GetMouseButton(1))
        {
            float ControlHorizontal = Input.GetAxis("Horizontal");
            float ControlVertical = Input.GetAxis("Vertical");

            obj.rigidbodyMain.transform.Rotate(Vector3.up * ControlHorizontal * 5);
            obj.rigidbodyMain.transform.Rotate(Vector3.right * ControlVertical * 5);
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
            obj.rigidbodyMain.transform.Rotate(ang);
        }
    }
    IEnumerator ButtonDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        testWarping = false;
    }
    public virtual void Move()
    {
        if (Input.GetKey("space") && obj.rigidbodyMain != null)
        {
            obj.rigidbodyMain.linearVelocity = Vector3.zero;
            val2 = val = 0;
            return;
        }
        if (Input.GetKey("r") && obj.rigidbodyMain != null)
        {
            if (!testWarping)
            {
                Sector ffSector = null;
                if (testSectorId <= 100)
                {
                    ffSector = SpaceManager.sectors.Find(x => x.galaxyId == LocalClient.galaxyId && x.systemId == LocalClient.systemId && x.id == testSectorId);
                }
                else
                {
                    testSectorId = 0;
                }
                if (ffSector == null)
                {
                    testSectorId = 0;
                }

                Debug.Log($"Warping to sector ({LocalClient.galaxyId} {LocalClient.systemId} {testSectorId})");
                LocalClient.ControlledObject.WarpSystem(LocalClient.SpaceSystem, testSectorId);
                testSectorId++;
                testWarping = true;
                StartCoroutine("ButtonDelayed");
            }
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

        obj.rigidbodyMain.AddForce(obj.transform.forward * maxSpeed * val2);
        string zn = "";
        if (val2 < 0)
        {
            zn = "-";
        }
        // MainHud mainHud = GameManager.canvasController.mainHud;
        currentSpeed = (int)(obj.rigidbodyMain.linearVelocity.magnitude);
    }
}