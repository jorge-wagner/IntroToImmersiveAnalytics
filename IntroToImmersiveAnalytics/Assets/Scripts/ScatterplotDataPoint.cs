using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterplotDataPoint : MonoBehaviour
{

    public string pointName;
    public string dataClass;

    public TMPro.TMP_Text textLabel;
    public Color pointColor;


    public bool showTextLabel = false;


    private float colorDelta = 0.4f;



    public void Select()
    {

    }

    public void Unselect()
    {

    }


    public void Highlight()
    {
        textLabel.enabled = true;
        this.GetComponent<Renderer>().material.color = new Color(pointColor.r + colorDelta, pointColor.g + colorDelta, pointColor.b + colorDelta, pointColor.a);


    }

    public void Unhighlight()
    {
        textLabel.enabled = false;
        this.GetComponent<Renderer>().material.color = new Color(pointColor.r, pointColor.g, pointColor.b, pointColor.a);


    }





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(textLabel.enabled)
            transform.LookAt(Camera.main.transform); // so that the text label, if visible, always faces the camera

    }
}
