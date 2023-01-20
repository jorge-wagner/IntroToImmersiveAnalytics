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


    // A reusable method to create an IATK CSVDataSource object.
    CSVDataSource createCSVDataSource(string data)
    {
        CSVDataSource dataSource;
        dataSource = gameObject.AddComponent<CSVDataSource>();
        dataSource.load(data, null);
        return dataSource;
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
                             setSingleColor(Color.white);
                             
                             //setSize(myCSVDataSource["Base"].Data).
                             //setColors(myCSVDataSource["Time"].Data.Select(x => g.Evaluate(x)).ToArray());

        // Use the "IATKUtil" class to get the corresponding Material mt 
        Material mt = IATKUtil.GetMaterialFromTopology(AbstractVisualisation.GeometryType.Points);
        mt.SetFloat("_MinSize", 0.025f);
        mt.SetFloat("_MaxSize", 0.025f);

        // Create a view builder with the point topology
        View view = vb.updateView().apply(gameObject, mt);


    }


}
