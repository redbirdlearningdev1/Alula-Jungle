using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphObject : MonoBehaviour
{
    // x-axis
    public Transform x_axis_parent;
    // y-axis
    public Transform y_axis_parent;

    public GameObject X_axis_indicator;
    public GameObject Y_axis_indicator;

    // graph points
    public Transform graph_point_container;
    public GameObject graph_point;

    void Awake()
    {
        

        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(1, 1));
        points.Add(new Vector2(2, 2));
        points.Add(new Vector2(3, 3));
        points.Add(new Vector2(4, 4));
        points.Add(new Vector2(5, 5));

        SetPoints(points);
    }

    public void CreateGraph(StudentPlayerData playerData)
    {
        // only create graph if there are enough data points
        if (playerData.overallMasteryPerGame.Count > 1)
        {
            // set y axis
            List<string> indicators = new List<string>();
            int yMax = Mathf.CeilToInt(playerData.overallMasteryPerGame[playerData.overallMasteryPerGame.Count - 1]) + 1;
            for (int i = 0; i < yMax; i++)
            {
                indicators.Add(i.ToString());
            }
            SetYAxis(indicators);


            SetXAxis(10);
        }
    }

    public void SetPoints(List<Vector2> points)
    {
        foreach (Vector2 point in points)
        {
            GameObject newPoint = Instantiate(graph_point, graph_point_container);
            newPoint.transform.localPosition = point;
        }
    }

    public void SetXAxis(int indicators)
    {
        for (int i = 0; i < indicators; i++)
        {
            GameObject indicator = Instantiate(X_axis_indicator, x_axis_parent);
        }
    }

    public void SetYAxis(List<string> indicators)
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            GameObject indicator = Instantiate(Y_axis_indicator, y_axis_parent);
            indicator.GetComponentInChildren<TextMeshProUGUI>().text = indicators[i];
        }
    }
}
