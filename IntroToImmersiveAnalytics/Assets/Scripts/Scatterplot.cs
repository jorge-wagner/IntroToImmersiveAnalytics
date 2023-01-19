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




        List<Dictionary<string, object>> data = CSVReader.Read(datasetPath);


        for (var i = 0; i < data.Count; i++)
        {
  
            float x = 0.05f * System.Convert.ToSingle(data[i]["x"]);

            float y = 0.05f * System.Convert.ToSingle(data[i]["y"]);

            float z = 0.05f * System.Convert.ToSingle(data[i]["z"]);
            
            ScatterplotDataPoint newDataPoint = Instantiate(pointPrefab, new Vector3(x, y, z), Quaternion.identity).
                GetComponent<ScatterplotDataPoint>();

            newDataPoint.transform.position += this.transform.position;
            newDataPoint.transform.parent = this.transform;
            newDataPoint.gameObject.name = data[i]["Name"].ToString();

            newDataPoint.dataClass = data[i]["Party"].ToString();

            newDataPoint.textLabel.text = data[i]["Name"].ToString() + " (" + newDataPoint.dataClass + ")";

            Color newColor = new Color();
            ColorUtility.TryParseHtmlString(data[i]["alternativeColorMap"].ToString(), out newColor);
            newColor.a = 1f;
            newDataPoint.GetComponent<Renderer>().material.color = newColor;
            newDataPoint.pointColor = newColor;

            scatterplotPoints.Add(newDataPoint);
        }


        // ADJUST COLLIDER BOX 


    }



    public void DumpPoints()
    {
        foreach (ScatterplotDataPoint point in scatterplotPoints)
        {
            Destroy(point.gameObject);
        }
    }
}