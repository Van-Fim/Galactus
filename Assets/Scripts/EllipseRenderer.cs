using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EllipseRenderer : MonoBehaviour
{
    public LineRenderer lr;
    [Range(3, 128)]
    public int segments;
    public Ellipse ellipse;

    public SolarObject parentObject;
    public SolarObject solarObject;

    void Awake()
    {
        
    }
    public void CalculateEllipse()
    {
        if (parentObject == null)
        {
            return;
        }
        float minlineWidth = 100f;
        if (parentObject.GetType() == typeof(SolarObject))
        {
            minlineWidth = 50f;
        }
        float lineWidth = minlineWidth;
        lineWidth = CameraManager.planetCamera.curCamera.orthographicSize / 500;
        if (lineWidth < minlineWidth)
        {
            lineWidth = minlineWidth;
        }
        Vector3[] points = new Vector3[segments + 1];
        Vector3 position = parentObject.solarController.transform.localPosition;
        Vector3 curPosition = solarObject.solarController.transform.localPosition;
        for (int i = 0; i < segments; i++)
        {
            Vector2 position2D = ellipse.Evaluate((float)i / (float)segments);
            DebugConsole.Log(position2D, false, null, "points");
            points[i] = new Vector3(position.x + position2D.x, 0f, position.y + position2D.y);
        }
        points[segments] = points[0];
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = segments + 1;
        lr.material.SetColor("_Color", solarObject.GetOrbitColor());
        lr.SetPositions(points);
    }

    void OnValidate()
    {
        if (Application.isPlaying && lr != null)
        {
            CalculateEllipse();
        }
    }
}
