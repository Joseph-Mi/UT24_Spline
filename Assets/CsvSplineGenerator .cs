using UnityEngine;
using BezierSolution;
using System.IO;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CsvSplineGenerator : MonoBehaviour
{
    public BezierSpline spline; // Reference to the spline
    public string csvFileName = "Data/oval_points.csv";
    public bool autoConstructSpline = true; // Auto construct smooth curve

    private List<Vector3> points = new List<Vector3>();

    private void OnValidate()
    {
        // Ensure that a BezierSpline component exists
        if (spline == null)
        {
            spline = gameObject.GetComponent<BezierSpline>();

            // Add BezierSpline if not attached already
            if (spline == null)
            {
                spline = gameObject.AddComponent<BezierSpline>();
            }
        }

        GenerateSplineFromCsv();
    }

    private void GenerateSplineFromCsv()
    {
        string fullPath = Path.Combine(Application.dataPath, csvFileName); // Correct file path

        if (LoadPointsFromCsv(fullPath))
        {
            // Remove all existing points
            while (spline.Count > 0)
            {
                spline.RemovePointAt(0);
            }

            // Add new control points from CSV data
            foreach (Vector3 point in points)
            {
                spline.InsertNewPointAt(spline.Count, point);
            }

            // Set handle modes for smoother curves
            for (int i = 0; i < spline.Count; i++)
            {
                spline[i].handleMode = BezierPoint.HandleMode.Mirrored;
                Debug.Log($"Point {i}: {spline[i].position}");
            }

            // Auto construct the spline for smoother transitions
            if (autoConstructSpline)
            {
                spline.AutoConstructSpline();
            }

            // Close the loop if the first and last points are the same
            if (points.Count > 1 && points[0] == points[points.Count - 1])
            {
                spline.Loop = true;
            }

            spline.Refresh(); // Refresh the spline so it updates
        }
        else
        {
            Debug.LogError("Failed to load points from CSV file.");
        }
    }

    private bool LoadPointsFromCsv(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Error reading CSV file: Could not find file " + filePath);
            return false;
        }

        points.Clear();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                bool isFirstLine = true;

                while ((line = sr.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        // Skip header line
                        isFirstLine = false;
                        continue;
                    }

                    string[] values = line.Split(',');

                    if (values.Length == 3 && !string.IsNullOrWhiteSpace(values[0]) && !string.IsNullOrWhiteSpace(values[1]) && !string.IsNullOrWhiteSpace(values[2]))
                    {
                        float x = float.Parse(values[0]);
                        float y = float.Parse(values[1]);
                        float z = float.Parse(values[2]);
                    
                        points.Add(new Vector3(x, y, z));
                    }
                }
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading CSV file: " + e.Message);
            return false;
        }
    }

    // Draw the spline in the Scene view using Gizmos
    private void OnDrawGizmos()
    {
        if (spline == null || spline.Count == 0)
            return;

        Gizmos.color = Color.yellow;
        Vector3 previousPoint = spline.GetPoint(0f);

        // Draw the spline as a series of lines
        for (int i = 1; i <= 100; i++)
        {
            float t = i / 100f;
            Vector3 currentPoint = spline.GetPoint(t);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }
}