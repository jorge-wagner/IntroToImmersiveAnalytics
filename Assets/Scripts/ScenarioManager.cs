using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public bool autoAdjustOnStart = true;
    public bool adjustmentComplete = false;
    public bool useRealDeskPosition = true;
    public GameObject table;
    public GameObject redCube; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (autoAdjustOnStart && !adjustmentComplete)
        {
            adjustmentComplete = adjustScenario();
        }
    }


    public bool adjustScenario()
    {
        if (useRealDeskPosition)
        {
            float deskThickness = 0.015f;
            //Transform deskPos = GetDeskTransformFromOVRSceneManager();
            GameObject deskRef = GameObject.Find("RealDeskSurface(Clone)"); // Looks for the object created automatically by OVRSceneManager to indicate the transform of the real desk, if found

            if (deskRef != null)
            {
                table.transform.rotation = Quaternion.Euler(0, deskRef.transform.rotation.eulerAngles.y, 0);
                table.transform.localScale = new Vector3(deskRef.transform.localScale.x, deskThickness, deskRef.transform.localScale.y);
                table.transform.position = deskRef.transform.position - new Vector3(0, table.transform.localScale.y / 2f, 0);

                redCube.transform.position = new Vector3(table.transform.position.x, table.transform.position.y + redCube.transform.localScale.y / 2f, table.transform.position.z);

                return true;
            }
            else
            {
                return false;
            }
        }
        else return true;  

    }



    /*
    /// <summary>
    /// When the Scene has loaded, instantiate all the wall and furniture items.
    /// OVRSceneManager creates proxy anchors, that we use as parent tranforms for these instantiated items.
    /// ADAPTED FROM: https://github.com/oculus-samples/Unity-TheWorldBeyond/blob/main/Assets/Scripts/WorldBeyondManager.cs
    /// AND: https://github.com/oculus-samples/Unity-TheWorldBeyond/blob/main/Assets/Scripts/VirtualRoom.cs
    /// </summary>
    Transform GetDeskTransformFromOVRSceneManager()
    {
        try
        {
            OVRSceneAnchor[] sceneAnchors = FindObjectsOfType<OVRSceneAnchor>();

            for (int i = 0; i < sceneAnchors.Length; i++)
            {
                OVRSceneAnchor instance = sceneAnchors[i];
                OVRSemanticClassification classification = instance.GetComponent<OVRSemanticClassification>();

                if (classification.Contains(OVRSceneManager.Classification.Desk))
                {
                    Debug.Log("DESK POS: " + instance.transform.GetChild(0).position);
                    Debug.Log("DESK ROT: " + instance.transform.GetChild(0).rotation);
                    Debug.Log("DESK SCL: " + instance.transform.GetChild(0).localScale);

                    return instance.transform.GetChild(0);
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }


    */
}
