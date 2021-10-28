using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleSlider : MonoBehaviour
{
    float max = 200;
    float min = 40;

    float currentValue;
    public string dataName;


    public Image image;
    public Text progress;
    public Text actualValue;
    public Text title;

    bool up = true;
    int frameCount = 0;

    void Start()
    {
        currentValue = min;
        title.text = dataName;
    }

    void Update()
    {
        if (frameCount % 40 == 0)
        {
            UpdateValue();
        }

        frameCount++;
    }

    void AddOne()
    {
        currentValue++;
    }

    void MinusOne()
    {
        currentValue--;
    }


    void UpdateValue()
    {
        if (currentValue >= max)
        {
            up = false;
        }
        else if (currentValue <= min)
        {
            up = true;
        }

        if (up)
        {
            AddOne();
        }
        else
        {
            MinusOne();
        }

        float val = (currentValue - min) / (max - min);
        image.fillAmount = val;
        var perc = (int)(image.fillAmount * 100) + "%";
        progress.text = perc;

        actualValue.text = currentValue + " kWh/m2";
    }
}
