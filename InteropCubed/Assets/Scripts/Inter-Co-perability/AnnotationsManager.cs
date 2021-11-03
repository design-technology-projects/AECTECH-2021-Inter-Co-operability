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
    public List<AnnotationObject> AnnotationsObjectStorage;
    public List<AnnotationIconSetter> AnnotationIconStorage;
    public List<GameObject> listAnnotationItems;

    public void AddToStorage(AnnotationObject toAdd, AnnotationIconSetter theIcon)
    {
        annotationUIButton.SetActive(true);

        AnnotationsObjectStorage.Add(toAdd);
        AnnotationIconStorage.Add(theIcon);

        var annotationGameObjects = Instantiate(listAnnotationItem, listAnnotationItem.transform.parent);
        var name = toAdd.theObject.name.Substring(0, Mathf.Min(toAdd.theObject.name.Length, 25));
        annotationGameObjects.name = name;

        var button = annotationGameObjects.GetComponentInChildren<UnityEngine.UI.Button>();
        button.onClick.AddListener(() =>
        {
            theIcon.SelectObject();
            listAnnotationItem.transform.parent.parent.GetComponent<ToggleGameobject>().ToggleGameObject(false);
        });

        var visibilityToggle = annotationGameObjects.GetComponentInChildren<UnityEngine.UI.Toggle>();
        theIcon.visibilityToggle = visibilityToggle;

        visibilityToggle.onValueChanged.AddListener((a) =>
        {
            theIcon.ToggleVisibility(!a);
        });

        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
        annotationGameObjects.SetActive(true);
        listAnnotationItems.Add(annotationGameObjects);

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
        AnnotationsObjectStorage = new List<AnnotationObject>();
        //AnnotationsObjectStorage = new List<Annotation>();
        AnnotationIconStorage = new List<AnnotationIconSetter>();
        listAnnotationItems = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
