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

    public TextMeshProUGUI last_x_indicator;
    public TextMeshProUGUI last_y_indicator;

    // graph points
    public Transform graph_point_container;
    public GameObject graph_point;

    void Awake()
    {
    
    }

    public void CreateGraph(StudentPlayerData playerData)
    {
        // only create graph if there are enough data points
        if (playerData.overallMasteryPerGame.Count > 1)
        {
            RectTransform rect = graph_point_container.GetComponent<RectTransform>();
            // print ("width: " + rect.rect.width);
            // print ("height: " + rect.rect.height);

            // set y axis
            List<string> yIndicators = new List<string>();
            int yMax = Mathf.CeilToInt(playerData.overallMasteryPerGame[playerData.overallMasteryPerGame.Count - 1]);
            for (int i = 0; i <= yMax; i++)
            {
                yIndicators.Add(i.ToString());
            }
            SetYAxis(yIndicators, rect.rect.width);


            // set x axis
            List<string> xIndicators = new List<string>();
            int xMax = Mathf.CeilToInt(playerData.overallMasteryPerGame.Count);
            for (int i = 0; i <= xMax; i++)
            {
                if (true)
                    xIndicators.Add(i.ToString());
            }
            SetXAxis(xIndicators, rect.rect.height);

            // get graph position for each point
            List<Vector2> graphPos = new List<Vector2>();
            int max = playerData.overallMasteryPerGame.Count - 1;
            int count = 0;
            foreach (var point in playerData.overallMasteryPerGame)
            {
                float x = Mathf.Lerp(0f, rect.rect.width, ((float)count / (float)(max + 1)));
                float y = Mathf.Lerp(0f, rect.rect.height, (playerData.overallMasteryPerGame[count] / (playerData.overallMasteryPerGame[max] + 0.5f)));
                graphPos.Add(new Vector2(x, y));                
                count++;
            }
            SetPoints(graphPos);
        }
    }

    public void SetPoints(List<Vector2> points)
    {
        // remove old points
        foreach (Transform child in graph_point_container)
        {
            Destroy(child.gameObject);
        }

        // add new points
        List<GameObject> graphPoints = new List<GameObject>();
        foreach (Vector2 point in points)
        {
            GameObject newPoint = Instantiate(graph_point, graph_point_container);
            newPoint.transform.localPosition = point;
            graphPoints.Add(newPoint);
        }
        
        // // add lines between points
        // Vector3 lineOffset = new Vector3(7.48f, 10.08f, -90f);
        // int i = 1;
        // foreach (Transform child in graph_point_container)
        // {
        //     if (i < points.Count)
        //     {
        //         // LineRenderer lineRenderer = child.gameObject.AddComponent<LineRenderer>();
        //         // lineRenderer.positionCount = 2;
        //         // lineRenderer.startWidth = 0.05f;
        //         // lineRenderer.endWidth = 0.05f;
        //         // lineRenderer.startColor = Color.white;
        //         // lineRenderer.endColor = Color.white;
        //         // lineRenderer.SetPosition(0, graphPoints[i - 1].transform.position + lineOffset);
        //         // lineRenderer.SetPosition(1, graphPoints[i].transform.position + lineOffset);
        //     }
        //     i++;
        // }
    }

    public void SetXAxis(List<string> indicators, float lineLength)
    {
        // remove old indicators
        foreach (Transform child in x_axis_parent)
        {
            Destroy(child.gameObject);
        }

        // add new indicators
        for (int i = 0; i < indicators.Count - 1; i++)
        {
            GameObject indicator = Instantiate(X_axis_indicator, x_axis_parent);
            indicator.GetComponentInChildren<TextMeshProUGUI>().text = indicators[i];
            indicator.GetComponentInChildren<GraphLine>().GetComponent<RectTransform>().sizeDelta = new Vector2(3f, lineLength + 10f);
        }
        last_x_indicator.text = indicators[indicators.Count - 1];
    }

    public void SetYAxis(List<string> indicators, float lineLength)
    {
        // remove old indicators
        foreach (Transform child in y_axis_parent)
        {
            Destroy(child.gameObject);
        }

        // add new indicators
        for (int i = 0; i < indicators.Count - 1; i++)
        {
            GameObject indicator = Instantiate(Y_axis_indicator, y_axis_parent);
            indicator.GetComponentInChildren<TextMeshProUGUI>().text = indicators[i];
            indicator.GetComponentInChildren<GraphLine>().GetComponent<RectTransform>().sizeDelta = new Vector2(lineLength + 10f, 3f);
        }
        last_y_indicator.text = indicators[indicators.Count - 1];
    }
}
