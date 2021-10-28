using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleSlider : MonoBehaviour
{
    public DataHandler dataHandler;

    public float min;
    public float max;
    public float currentValue;
    public float averageValue;
    public string units;
    public float distanceData;

    public string dataName;
    

    public Image image;
    public Text progress;
    public Text actualValue;
    public Text title;
    public Text distance;


    void OnEnable()
    {
        min = dataHandler.GetMin(dataName.ToLower());
        max = dataHandler.GetMax(dataName.ToLower());
        averageValue = dataHandler.GetAverage(dataName.ToLower());
        units = dataHandler.GetUnit(dataName.ToLower());
        currentValue = dataHandler.GetLocalValue(dataName.ToLower());

        distance.text = Math.Round(dataHandler.GetDistanceToDataPoint(dataName.ToLower()), 2).ToString() + " ";
        title.text = dataName;
    }

    void Update()
    {
        // Quit reading the data while it is still loading from external files.
        if (dataHandler.receiving)
        {
            return;
        }

        currentValue = dataHandler.GetLocalValue(dataName.ToLower());

        float val = (currentValue - min) / (max - min);
        image.fillAmount = val;

        var perc = (int)((currentValue / averageValue) * 100) + "%";
        progress.text = perc;

        actualValue.text = currentValue + " " + units;
    }

}


