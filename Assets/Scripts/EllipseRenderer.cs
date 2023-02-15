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
        lr = GetComponent<LineRenderer>();
        CalculateEllipse();
    }
    public void CalculateEllipse()
    {
        if (parentObject == null)
        {
            return;
        }
        float minlineWidth = 10f;
        if (parentObject.GetType() == typeof(SolarObject))
        {
            minlineWidth = 5f;
        }
        float lineWidth = minlineWidth;
        lineWidth = CameraManager.planetCamera.curCamera.orthographicSize / 500;
        if (lineWidth < minlineWidth)
        {
            lineWidth = minlineWidth;
        }

        Vector3[] points = new Vector3[segments + 1];
        Vector2 position = parentObject.solarController.transform.localPosition;
        Vector2 curPosition = solarObject.solarController.transform.localPosition;
        for (int i = 0; i < segments; i++)
        {
            Vector2 position2D = ellipse.Evaluate((float)i / (float)segments);
            points[i] = new Vector3(position.x + position2D.x, position.y + position2D.y, 0f);
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
