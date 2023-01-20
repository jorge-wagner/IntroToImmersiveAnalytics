using System.Collections;
using System.Collections.Generic;
using IATK;
using UnityEngine;

public class ScatterplotPlotterWithIATK : MonoBehaviour
{
    // Start is called before the first frame update
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
        TextAsset myDataSource = Resources.Load(datasetPath) as TextAsset;
        CSVDataSource myCSVDataSource;
        myCSVDataSource = createCSVDataSource(myDataSource.text);

        // Create a view builder with the point topology
        ViewBuilder vb = new ViewBuilder(MeshTopology.Points, "IATK Scatterplot").
                             initialiseDataView(myCSVDataSource.DataCount).
                             setDataDimension(myCSVDataSource["x"].Data, ViewBuilder.VIEW_DIMENSION.X).
                             setDataDimension(myCSVDataSource["y"].Data, ViewBuilder.VIEW_DIMENSION.Y).
                             setDataDimension(myCSVDataSource["z"].Data, ViewBuilder.VIEW_DIMENSION.Z).
                             setColors(getListOfColorsFromListOfHexStrings(myCSVDataSource, "alternativeColorMap"));                    

        // Use the "IATKUtil" class to get the corresponding Material mt 
        Material mt = IATKUtil.GetMaterialFromTopology(AbstractVisualisation.GeometryType.Spheres);
        mt.SetFloat("_MinSize", 0.025f);
        mt.SetFloat("_MaxSize", 0.025f);

        // Create a view builder with the point topology
        View view = vb.updateView().apply(gameObject, mt);

        view.transform.position -= new Vector3(0.5f, 0.5f, 0.5f); // compensating for the fact that natively the center of the view object is the lower left corner of the visualization and not its centroid.
    }



    // A reusable method to create an IATK CSVDataSource object.
    CSVDataSource createCSVDataSource(string data)
    {
        CSVDataSource dataSource;
        dataSource = gameObject.AddComponent<CSVDataSource>();
        dataSource.load(data, null);
        return dataSource;
    }


    // Improvised function to retrieve data from a column comprising color strings in hex format 
    Color[] getListOfColorsFromListOfHexStrings(CSVDataSource csvds, string colorColumnIdentifier)
    {
        float[] iatk_float_data = csvds[colorColumnIdentifier].Data;
        Color[] newColorList = new Color[iatk_float_data.Length];
        int i = 0;

        foreach (float f in iatk_float_data)
        {
            Color newCol;
            if (ColorUtility.TryParseHtmlString(csvds.getOriginalValue(f, colorColumnIdentifier).ToString(), out newCol))
            {
                newColorList[i++] = newCol;
            }
        }

        return newColorList;
    }

}
