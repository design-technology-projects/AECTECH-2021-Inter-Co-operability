using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotationsManager : MonoBehaviour
{
    public static AnnotationsManager instance;

    public GameObject listAnnotationItem;
    public GameObject annotationUIButton;

    // list to be populated with all annotation objects.
    // both the ones I make and the ones comming from Speckle
    public List<AnnotationsHandler.AnnotationObject> _AnnotationsObjectStorage;
    public List<Annotation> AnnotationsObjectStorage;
    public List<AnnotationIconSetter> AnnotationIconStorage;

    public void AddToStorage(AnnotationsHandler.AnnotationObject toAdd, AnnotationIconSetter theIcon)
    {
        annotationUIButton.SetActive(true);

        _AnnotationsObjectStorage.Add(toAdd);
        AnnotationsObjectStorage.Add( (Annotation) toAdd);
        AnnotationIconStorage.Add(theIcon);

        var nInList = Instantiate(listAnnotationItem, listAnnotationItem.transform.parent);
        var name = toAdd.theObject.name.Substring(0, Mathf.Min(toAdd.theObject.name.Length, 25));
        nInList.name = name;

        var button = nInList.GetComponentInChildren<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => {
            theIcon.SelectObject();
            listAnnotationItem.transform.parent.parent.GetComponent<ToggleGameobject>().ToggleGameObject(false);
        });

        var visibilityToggle = nInList.GetComponentInChildren<UnityEngine.UI.Toggle>();
        theIcon.visibilityToggle = visibilityToggle;
        visibilityToggle.onValueChanged.AddListener((a) => {
            theIcon.ToggleVisibility(!a);});

        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
        nInList.SetActive(true);

    }


    bool allvisibility = false;
    public void ToggleAllIconVisibility()
    {
        allvisibility = !allvisibility;
        foreach (var icon in AnnotationIconStorage)
        {
            icon.ToggleVisibility(allvisibility);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        _AnnotationsObjectStorage = new List<AnnotationsHandler.AnnotationObject>();
        AnnotationsObjectStorage = new List<Annotation>();
        AnnotationIconStorage = new List<AnnotationIconSetter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
