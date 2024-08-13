using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOController : MonoBehaviour
{
    public SpaceObject obj;

    private float val = 0;
    private float val2 = 0;

    private int maxSpeed = 300;
    private int rotationSpeed = 150;
    private float velocity = 100;
    private bool isHyperMode = false;

    public static bool blocked = false;
    public static int currentSpeed = 0;
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
            int newMaxSpeed = int.Parse(paramsNode.GetValue("maxspeed"));
            int newRotationSpeed = int.Parse(paramsNode.GetValue("rotationspeed"));
            int newVelocity = int.Parse(paramsNode.GetValue("velocity"));

            this.maxSpeed = newMaxSpeed;
            this.rotationSpeed = newRotationSpeed;
            this.velocity = newVelocity;
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

    public virtual void Move()
    {
        if (Input.GetKey("space") && obj.rigidbodyMain != null)
        {
            obj.rigidbodyMain.velocity = Vector3.zero;
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

        obj.rigidbodyMain.AddForce(obj.transform.forward * maxSpeed * val2);
        string zn = "";
        if (val2 < 0)
        {
            zn = "-";
        }
        // MainHud mainHud = GameManager.canvasController.mainHud;
        currentSpeed = (int)(obj.rigidbodyMain.velocity.magnitude);
    }
}