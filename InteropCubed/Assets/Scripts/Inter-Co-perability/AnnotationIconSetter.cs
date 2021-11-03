using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationIconSetter : MonoBehaviour
{
    public AnnotationObject objectRef;
    public GameObject iconRef;
    public UnityEngine.UI.Toggle visibilityToggle;

    bool isOn = false;
    Unity.Reflect.Viewer.UI.UIProjectStateData oldData;
    public void SelectObject()
    {
        AnnotationsHandler.instance.RightSideBarController.ClickSelectButton();
        if (isOn)
        {
            AnnotationsHandler.instance.UIStateManager.setUIProjectStateData(oldData);
            AnnotationsHandler.instance.UIStateManager.ForceSendProjectStateChangedEvent();
            isOn = false;
        }
        else
        {
            Debug.Log("Selecting Object now with Id: " + objectRef.Id);

            var newPS = AnnotationsHandler.instance.UIStateManager.projectStateData;
            oldData = AnnotationsHandler.instance.UIStateManager.projectStateData;
            newPS.objectSelectionInfo.selectedObjects = new List<GameObject>() { objectRef.theObject };
            //newPS.filterGroupList = new List<string>(AnnotationsHandler.instance.UIStateManager.projectStateData.filterGroupList);
            //newPS.filterItemInfos = new List<Unity.Reflect.Viewer.UI.FilterItemInfo>(AnnotationsHandler.instance.UIStateManager.projectStateData.filterItemInfos);
            //newPS.filterSearchString = AnnotationsHandler.instance.UIStateManager.projectStateData.filterSearchString;
            AnnotationsHandler.instance.UIStateManager.setUIProjectStateData(newPS);
            AnnotationsHandler.instance.UIStateManager.ForceSendProjectStateChangedEvent();
            isOn = true;
        }

    }
    public void ToggleVisibility(bool a)
    {
        iconRef.SetActive(a);
        visibilityToggle.SetIsOnWithoutNotify(!a);
    }
}
