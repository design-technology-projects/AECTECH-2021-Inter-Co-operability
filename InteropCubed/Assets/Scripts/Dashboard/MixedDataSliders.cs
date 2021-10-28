using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MixedDataSliders : MonoBehaviour
{
    public DataHandler dataHandler;


    public List<string> dataNames;
    public List<GameObject> sliders;

    public List<float> min = new List<float>();
    public List<float> max = new List<float>();
    public List<float> curentValue = new List<float>();

    // Start is called before the first frame update
    void OnEnable()
    {

        // Read data point values based on the analysis name
        // Assign random values to the extra slider
        for (int i = 0; i < dataNames.Count; i++)
        {

            if (dataNames[i] == "random")
            {
                this.min.Add(0);
                this.max.Add(100);
                this.curentValue.Add(50);
            }
            else
            {
                this.min.Add(dataHandler.GetMin(dataNames[i].ToLower()));
                this.max.Add(dataHandler.GetMax(dataNames[i].ToLower()));
                this.curentValue.Add(dataHandler.GetLocalValue(dataNames[i].ToLower()));
                sliders[i].GetComponentInChildren<Text>().text = dataNames[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Quit reading the data while it is still loading from external files.
        if (dataHandler.receiving)
        {
            return;
        }


        // Read and update the slider
        for (int i = 0; i < dataNames.Count; i++)
        {

            if (dataNames[i] == "random")
            {

                this.curentValue[i] = curentValue[i] + (Random.Range(-1,1))*0.1f;

                if (this.curentValue[i] < min[i])  { this.curentValue[i] = max[i]; }
                if (this.curentValue[i] > max[i])  { this.curentValue[i] = min[i]; }

            }
            else
            {
                this.curentValue[i] = dataHandler.GetLocalValue(dataNames[i].ToLower());
            }

            sliders[i].GetComponent<UnityEngine.UI.Slider>().value = (curentValue[i] - min[i]) / (max[i] - min[i]);
        }
    }
}
