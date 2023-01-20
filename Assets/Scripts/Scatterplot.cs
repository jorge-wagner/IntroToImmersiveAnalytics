using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Scatterplot : MonoBehaviour
{

    public GameObject pointPrefab;
    List<ScatterplotDataPoint> scatterplotPoints = null;



    // Use this for initialization
    void Start()
    {
        LoadPoints("Data/legislature54");
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void LoadPoints(string datasetPath)
    {
        if(scatterplotPoints != null)
            DumpPoints();

        scatterplotPoints = new List<ScatterplotDataPoint>();

        List<Dictionary<string, object>> csvData = CSVReader.Read(datasetPath);


        for (var i = 0; i < csvData.Count; i++)
        {
  
            float x = 0.05f * System.Convert.ToSingle(csvData[i]["x"]);

            float y = 0.05f * System.Convert.ToSingle(csvData[i]["y"]);

            float z = 0.05f * System.Convert.ToSingle(csvData[i]["z"]);
            
            ScatterplotDataPoint newDataPoint = Instantiate(pointPrefab, new Vector3(x, y, z), Quaternion.identity).
                GetComponent<ScatterplotDataPoint>();

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = csvData[i]["Name"].ToString();

            newDataPoint.dataClass = csvData[i]["Party"].ToString();

            newDataPoint.textLabel.text = csvData[i]["Name"].ToString() + " (" + newDataPoint.dataClass + ")";

            Color newColor = new Color();
            ColorUtility.TryParseHtmlString(csvData[i]["alternativeColorMap"].ToString(), out newColor);
            newColor.a = 1f;
            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;

            scatterplotPoints.Add(newDataPoint);
        }


        // Should also adjust size of scatterplot collider box here based on points positions

    }



    public void DumpPoints()
    {
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            Destroy(point.gameObject);
        }
    }
}