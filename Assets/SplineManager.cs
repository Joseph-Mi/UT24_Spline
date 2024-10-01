using UnityEngine;
using BezierSolution;
using System.Collections.Generic;
public class SplineManager : MonoBehaviour
{
    public BezierSpline spline;
    void Start()
    {
        // Initialize the spline
        spline = gameObject.AddComponent<BezierSpline>();
        // Example: Create a spline with multiple points
        List<Vector3> points = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(2, 0, 0),
            new Vector3(3, -1, 0)
        };
        CreateSplineFromPoints(points);
        // Access or modify spline points programmatically
        Vector3 midPoint = spline.GetPoint(0.5f);
        Debug.Log("Point on spline: " + midPoint);
    }
    public void CreateSplineFromPoints(List<Vector3> points)
    {
        if (points.Count < 2)
        {
            Debug.LogError("At least two points are required to create a spline.");
            return;
        }
        // Initialize the spline with the correct number of points
        spline.Initialize(points.Count);
        // Set the positions of the points
        for (int i = 0; i < points.Count; i++)
        {
            spline[i].position = points[i];
        }
        // Auto construct the spline to create a smooth curve
        spline.autoConstructMode = SplineAutoConstructMode.Smooth;
        spline.autoCalculateNormals = true;
        spline.Refresh();
    }
    public void AddPointToSpline(Vector3 point)
    {
        int newIndex = spline.Count;
        spline.InsertNewPointAt(newIndex);
        spline[newIndex].position = point;
        spline.Refresh();
    }
    public Vector3 GetPointOnSpline(float t)
    {
        return spline.GetPoint(t);
    }
    public void RemoveLastPoint()
    {
        if (spline.Count > 2)
        {
            spline.RemovePointAt(spline.Count - 1);
            spline.Refresh();
        }
        else
        {
            Debug.LogWarning("Cannot remove point: spline must have at least two points.");
        }
    }
}