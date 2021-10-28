using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameobject : MonoBehaviour
{
    public bool canvasToggle = false;
    Canvas canvas;
    private void Start()
    {
        canvas = this.GetComponent<Canvas>();
    }
    public void ToggleGameObject()
    {
        if (canvasToggle)
        {
            canvas.enabled = !canvas.enabled;
        }
        else
            this.gameObject.SetActive(!this.gameObject.activeInHierarchy);
    }
    public void ToggleGameObject(bool active)
    {
        if (canvasToggle)
        {
            canvas.enabled = active;
        }
        else
            this.gameObject.SetActive(active);
    }
}
