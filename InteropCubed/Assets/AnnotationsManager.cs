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
    public List<AnnotationsHandler.AnnotationObject> AnnotationsObjectStorage;
    public List<AnnotationIconSetter> AnnotationIconStorage;

    public void AddToStorage(AnnotationsHandler.AnnotationObject toAdd, AnnotationIconSetter theIcon)
    {
        annotationUIButton.SetActive(true);

        AnnotationsObjectStorage.Add(toAdd);
        AnnotationIconStorage.Add(theIcon);
        var nInList = Instantiate(listAnnotationItem, listAnnotationItem.transform.parent);
        var name = toAdd.theObject.name.Substring(0, Mathf.Min(toAdd.theObject.name.Length, 25));
        nInList.name = name;
        var button = nInList.GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(()=> { theIcon.SelectObject();
            listAnnotationItem.transform.parent.parent.GetComponent<ToggleGameobject>().ToggleGameObject(false); });
        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
        nInList.SetActive(true);

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
        AnnotationsObjectStorage        = new List<AnnotationsHandler.AnnotationObject>();
        AnnotationIconStorage = new List<AnnotationIconSetter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
