using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wavecircle : MonoBehaviour
{
    public DataHandler dataHandler;

    public float min;
    public float max;
    public float currentValue;
    public float averageValue;
    public string units;

    public string dataName;

    public Text progress;
    public Text actualValue;
    public Text title;

    public Transform wave;
    public Transform s, e;


    private void OnEnable()
    {
        // Read data point values based on the analysis name
        min = dataHandler.GetMin(dataName.ToLower());
        max = dataHandler.GetMax(dataName.ToLower());
        averageValue = dataHandler.GetAverage(dataName.ToLower());
        units = dataHandler.GetUnit(dataName.ToLower());
        currentValue = dataHandler.GetLocalValue(dataName.ToLower());
        title.text = dataName;
    }

    // Update is called once per frame
    void Update()
    {
        // Quit reading the data while it is still loading from external files.
        if (dataHandler.receiving)
        {
            return;
        }

        //Get local data
        currentValue = dataHandler.GetLocalValue(dataName.ToLower());

        // The percentage value updated to animation
        float truePercentageValue =0.0f+ (currentValue - min) / (max - min) * 1f;
        wave.position = s.position + (e.position - s.position) * truePercentageValue;
        actualValue.text = Math.Round(currentValue, 2).ToString() + " " + units;

        // The value respective to the average value
        progress.text = Mathf.RoundToInt((currentValue / averageValue) * 100) + " %";
    }
}
