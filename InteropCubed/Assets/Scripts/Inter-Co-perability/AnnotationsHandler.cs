using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;
using Unity.Reflect.Data;
using Unity.Reflect.Viewer;
using System.Linq;
using TMPro;
using System;
using System.Threading.Tasks;

public class AnnotationsHandler : MonoBehaviour
{
    public static AnnotationsHandler instance;

    [Header("Scene references")]
    public Unity.Reflect.Viewer.UI.RightSideBarController RightSideBarController;
    public Unity.Reflect.Viewer.UI.UIStateManager UIStateManager;

    public SpeckleAnnotationManager speckleAnnotationManager;

    public GameObject AnnotationsIconPrefab;
    private GameObject AnnotationsIcon;
    public GameObject AnnotationsWindow;

    public AnnotationParameters[] annotationParams;

    public AnnotationObject SelectedObject;

    private string[] groupKeys = new string[] {
           "Identity Data",
           "Structural",
           "Constraints",
           "Text",
           "Other",
           "Phasing",
           "Dimensions",
           "VerticalGrid",
           "Materials And Finishes",
           "Construction",
           "General",
           "Data",
           "IFC Parameters",
           "Vertical Mullions"
    };

    public TMP_InputField inputCommentField;
    public TMP_InputField inputAssigneeField;

    [HideInInspector] public ToggleGameobject inputCommentToggleClas;
    private GameObject parentAnnotationIcons;
    public void AddAnnotation()
    {
        Debug.Log("<color=yellow> Adding annotation to selected Gameobject! </color>");
        // here we will intatiate/enable the annotation window /--- happening of the button
    }
    public void AddAnnotationText(string text)
    {
        Debug.Log("<color=yellow> Adding annotation text! </color>");
        Debug.LogFormat("<color=green> Comment: {0} </color>", text);
        SelectedObject.annotationComment = text;
    }
    public void AddAnnotationAssignee(string text)
    {
        Debug.Log("<color=yellow> Adding annotation Assignee! </color>");
        Debug.LogFormat("<color=green> Assignee: {0} </color>", text);
        SelectedObject.annotationAssignee = text;
    }
    public void AddAnnotationMEsh(GameObject annotationMesh)
    {
        Debug.Log("<color=yellow> Adding annotation Mesh! </color>");
        Debug.LogFormat("<color=green> Annotaton Mesh: {0} </color>", annotationMesh.name);
        SelectedObject.annotationMesh = annotationMesh;
    }

    public void SendAnnotations()
    {
        var storagelist = AnnotationsManager.instance.AnnotationsObjectStorage;

        if (SelectedObject != new AnnotationObject())
            if (!storagelist.Contains(SelectedObject))
            {
                //Here place the icon to indicate that annotation exists
                AnnotationsIcon = Instantiate(AnnotationsIconPrefab, parentAnnotationIcons.transform);
                var theSetter = AnnotationsIcon.GetComponent<AnnotationIconSetter>();
                theSetter.objectRef = SelectedObject;

                AnnotationsIcon.transform.SetPositionAndRotation(SelectedObject.theObject.transform.position, Quaternion.identity);
                var objectPos = SelectedObject.theObject.GetComponent<SyncObjectBinding>().bounds;
                AnnotationsIcon.transform.localPosition = objectPos.center + new Vector3(0, 20.0f, 0);
                var line = AnnotationsIcon.GetComponentInChildren<LineRenderer>();
                line.useWorldSpace = true;
                line.SetPosition(0, AnnotationsIcon.transform.GetChild(0).position);
                line.SetPosition(1, objectPos.center);
                AnnotationsManager.instance.AddToStorage(SelectedObject, theSetter);

                // speckle
                speckleAnnotationManager.SendAnnotation();
            }

    }
    public void ClearSelectedObject()
    {
        //var storagelist = AnnotationsManager.instance.AnnotationsObjectStorage;
        //if (SelectedObject != new AnnotationObject())
        //    if (!storagelist.Contains(SelectedObject))
        //        storagelist.Add(SelectedObject);
        SelectedObject = new AnnotationObject();
    }

    Metadata prevData;
    public void AssignSelectedObject(Metadata metadata, GameObject selectedGameobject)
    {
        prevData = metadata;
        ClearSelectedObject();

        Debug.Log("<color=blue> Checking if object has annotations </color>");
        var storageList = AnnotationsManager.instance.AnnotationsObjectStorage;
        foreach (var group in metadata.SortedByGroup())
        {
            foreach (var parameter in group.Value)
            {
                AnnotationObject receivedAnnotationObject = null;
                if (parameter.Key == "Id")
                {
                    Debug.LogFormat("<color=blue> parameter ID: {0} exist in group: {1} ! </color>", parameter.Key, group.Key);
                    if (storageList.Any(a => parameter.Value.value == a.Id.ToString()))
                    {
                        Debug.Log("<color=blue> parameter ID found in stored Annotations </color>");
                        if (storageList.Any(a => a.Document == group.Value["Document"].value))
                        {
                            Debug.Log("<color=blue> parameter ID and GROUP found in stored Annotations </color>");
                            receivedAnnotationObject = storageList.SingleOrDefault(a => a.Id.ToString() == parameter.Value.value);

                            Debug.LogFormat("<color=blue> Stored Annotation object selected with ID: {0} </color>", receivedAnnotationObject.Id.ToString());
                        }
                    }
                }
                if (receivedAnnotationObject != null)
                {
                    Debug.Log("<color=blue> Existing Annotation Found </color>");
                    SelectedObject = receivedAnnotationObject;
                    inputCommentField.SetTextWithoutNotify(SelectedObject.annotationComment);
                    inputAssigneeField.SetTextWithoutNotify(SelectedObject.annotationAssignee);

                    return;
                }
            }
        }

        Debug.Log("<color=yellow> Assigning selected object! </color>");

        SelectedObject = new AnnotationObject();
        foreach (var group in metadata.SortedByGroup())
        {
            if (annotationParams.Any(p => group.Key == groupKeys[(int)p.group]))
                foreach (var parameter in group.Value)
                {
                    if (annotationParams.Any(pp => pp.type == parameter.Key))
                    {
                        Debug.LogFormat("<color=grey> Assigning selected object's values: {0}, {1} </color>", parameter.Key, parameter.Value.value.ToString());
                        SelectedObject.values.Add(parameter.Key, parameter.Value.value.ToString());
                        SelectedObject.theObject = selectedGameobject;
                        //Debug
                        switch (parameter.Key)
                        {
                            case "Id":
                                {
                                    SelectedObject.Id = int.Parse(parameter.Value.value.ToString());
                                    break;
                                }
                            case "Category":
                                {
                                    SelectedObject.Category = parameter.Value.value.ToString();
                                    break;
                                }
                            case "Document":
                                {
                                    SelectedObject.Document = parameter.Value.value.ToString();
                                    break;
                                }
                            case "Comments":
                                {
                                    SelectedObject.Comments = parameter.Value.value.ToString();
                                    break;
                                }
                            case "Type Mask":
                                {
                                    SelectedObject.TypeMask = parameter.Value.value.ToString();
                                    break;
                                }
                            case "Phase Created":
                                {
                                    SelectedObject.PhaseCreated = parameter.Value.value.ToString();
                                    break;
                                }
                            case "Length":
                                {
                                    SelectedObject.Length = parameter.Value.value.ToString();
                                    break;
                                }
                            case "Area":
                                {
                                    SelectedObject.Area = parameter.Value.value.ToString();
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
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

        inputCommentToggleClas = AnnotationsWindow.GetComponent<ToggleGameobject>();

        parentAnnotationIcons = new GameObject();
        parentAnnotationIcons.name = "parentAnnotationIcons";
        parentAnnotationIcons.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        //speckleAnnotationManager = speckleManager.GetComponent<SpeckleAnnotationManager>();
    }

    public void ReceiveAnnotation()
    {
        ReceiveAsync();
    }

    public async Task ReceiveAsync()
    {
        // clean up old annotations
        CleanUpAnnotationStorage();


        Debug.Log("<color=green> Testing REceive now! </color>");
        await speckleAnnotationManager.Receive();
        StartCoroutine(ReceivedAnnotations());
    }
    private void CleanUpAnnotationStorage()
    {
        AnnotationsManager.instance.AnnotationsObjectStorage.ForEach(x => Destroy(x.annotationMesh));
        AnnotationsManager.instance.AnnotationsObjectStorage.ForEach(x => Destroy(x.theObject));
        if (AnnotationsManager.instance.AnnotationsObjectStorage != null) AnnotationsManager.instance.AnnotationsObjectStorage.Clear();

        AnnotationsManager.instance.AnnotationIconStorage.ForEach(x => Destroy(x.iconRef));
        if (AnnotationsManager.instance.AnnotationIconStorage != null) AnnotationsManager.instance.AnnotationIconStorage.Clear();

        AnnotationsManager.instance.listAnnotationItems.ForEach(x => Destroy(x));
    }

    public IEnumerator ReceivedAnnotations()
    {
        var metas = FindSceneObjectsOfType(typeof(Metadata)) as Metadata[];
        // example receiving
        foreach (var receivedAnnotation in speckleAnnotationManager.streamAnnotations)
        {
            Debug.LogFormat("<color=green> Testing REceive on {0}! </color>", receivedAnnotation.ElementRevitId);
            yield return null;
            GameObject found = null;
            for (int i = 0; i < metas.Length; i++)
            {
                foreach (var group in metas[i].SortedByGroup())
                    foreach (var parameter in group.Value)
                        if (parameter.Key == "Id")
                        {
                            if (parameter.Value.value == receivedAnnotation.ElementRevitId)
                            {
                                found = metas[i].gameObject; continue;
                            }
                        }
            }

            if (found != null)
            {
                Debug.Log("<color=green> Found obj! </color>");
                //var newPS = AnnotationsHandler.instance.UIStateManager.projectStateData;
                //newPS.objectSelectionInfo.selectedObjects = new List<GameObject>() { found };
                ////newPS.filterGroupList = new List<string>(AnnotationsHandler.instance.UIStateManager.projectStateData.filterGroupList);
                ////newPS.filterItemInfos = new List<Unity.Reflect.Viewer.UI.FilterItemInfo>(AnnotationsHandler.instance.UIStateManager.projectStateData.filterItemInfos);
                ////newPS.filterSearchString = AnnotationsHandler.instance.UIStateManager.projectStateData.filterSearchString;
                //AnnotationsHandler.instance.UIStateManager.setUIProjectStateData(newPS);
                //AnnotationsHandler.instance.UIStateManager.ForceSendProjectStateChangedEvent();

                var convertedAnnotation = (AnnotationObject)receivedAnnotation;
                convertedAnnotation.theObject = found;
                var _AnnotationsIcon = Instantiate(AnnotationsIconPrefab, parentAnnotationIcons.transform);
                var theSetter = _AnnotationsIcon.GetComponent<AnnotationIconSetter>();
                theSetter.objectRef = convertedAnnotation;
                _AnnotationsIcon.transform.SetPositionAndRotation(found.transform.position, Quaternion.identity);
                var objectPos = found.GetComponent<SyncObjectBinding>().bounds;
                _AnnotationsIcon.transform.localPosition = objectPos.center + new Vector3(0, 20.0f, 0);
                var line = _AnnotationsIcon.GetComponentInChildren<LineRenderer>();
                line.useWorldSpace = true;
                line.SetPosition(0, _AnnotationsIcon.transform.GetChild(0).position);
                line.SetPosition(1, objectPos.center);
                AnnotationsManager.instance.AddToStorage(convertedAnnotation, theSetter);
            }
        }
        yield break;
    }


    // Update is called once per frame
    void Update()
    {

    }
    [System.Serializable]
    public class AnnotationParameters
    {
        public enum groupType
        {
            IdentityData = 0,
            Structural = 1,
            Constraints = 2,
            Text = 3,
            Other = 4,
            Phasing = 5,
            Dimensions = 6,
            VerticalGrid = 7,
            MaterialsAndFinishes = 8,
            Construction = 9,
            General = 10,
            Data = 11,
            IFC_Parameters = 12,
            VerticalMullions = 13
        }
        public groupType group;
        public string type;

    }
    //[System.Serializable]
    //public class AnnotationObject
    //{
    //    public Dictionary<string, string> values = new Dictionary<string, string>();
    //    public string annotationComment;
    //    public string annotationAssignee;
    //    public GameObject annotationMesh;
    //    public GameObject theObject;

    //    public static explicit operator Annotation(AnnotationObject obj)
    //    {
    //        var SpeckleAnnotation = new Annotation()
    //        {
    //            ElementRevitId = obj.Id.ToString(),
    //            AnnotationId = RandomString(5),
    //            Assignee = obj.annotationAssignee,
    //            Message = obj.annotationComment
    //        };

    //        SpeckleAnnotation.Mesh = obj.annotationMesh == null ? null : GameobjectToBaseMesh(obj.annotationMesh);

    //        return SpeckleAnnotation;
    //    }
    //    // visible debug values _ class functions with dictionary
    //    public int Id;
    //    public string Category;
    //    public string Document;
    //    public string Comments;
    //    public string Assignee;
    //    public string TypeMask;
    //    public string PhaseCreated;
    //    public string Length;
    //    public string Area;

    //    public static Speckle.Core.Models.Base GameobjectToBaseMesh(GameObject obj)
    //    {
    //        var converter = new Objects.Converter.Unity.ConverterUnity();
    //        var convertedObjj = AnnotationsHandler.instance.sender.RecurseTreeToNative(obj);
    //        return convertedObjj;
    //    }
    //    public static string RandomString(int length)
    //    {
    //        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    //        return new string(Enumerable.Repeat(chars, length)
    //          .Select(s => s[UnityEngine.Random.Range(0, chars.Length - 1)]).ToArray());
    //    }

    //}
}


[System.Serializable]
public class AnnotationObject
{
    public Dictionary<string, string> values = new Dictionary<string, string>();
    public string annotationComment;
    public string annotationAssignee;
    public GameObject annotationMesh;
    public GameObject theObject;

    public static explicit operator Annotation(AnnotationObject obj)
    {
        var SpeckleAnnotation = new Annotation()
        {
            ElementRevitId = obj.Id.ToString(),
            AnnotationId = RandomString(5),
            Assignee = obj.annotationAssignee,
            Message = obj.annotationComment
        };

        SpeckleAnnotation.Mesh = obj.annotationMesh == null ? null : GameobjectToBaseMesh(obj.annotationMesh);

        return SpeckleAnnotation;
    }


    // visible debug values _ class functions with dictionary
    public int Id;
    public string Category;
    public string Document;
    public string Comments;
    public string Assignee;
    public string TypeMask;
    public string PhaseCreated;
    public string Length;
    public string Area;

    public static Speckle.Core.Models.Base GameobjectToBaseMesh(GameObject obj)
    {
        var converter = new Objects.Converter.Unity.ConverterUnity();
        var convertedObjj = AnnotationUtils.RecurseTreeToNative(obj);
        return convertedObjj;
    }
    public static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[UnityEngine.Random.Range(0, chars.Length - 1)]).ToArray());
    }

}


public class Annotation : Speckle.Core.Models.Base
{
    public string ElementRevitId { get; set; }
    public string AnnotationId { get; set; }
    public string Assignee { get; set; }
    public string Message { get; set; }
    public Speckle.Core.Models.Base Mesh { get; set; }

    public static explicit operator AnnotationObject(Annotation obj)
    {
        var ReflectAnnotation = new AnnotationObject()
        {
            Id = int.Parse(obj.ElementRevitId),
            annotationComment = obj.Message,
            annotationAssignee = obj.Assignee
        };

        ReflectAnnotation.values = new Dictionary<string, string>();
        ReflectAnnotation.values.Add("Id", obj.ElementRevitId);

        return ReflectAnnotation;
    }
}

public class AnnotationReceivingExample
{
    public static List<Annotation> receivedAnnotations = new List<Annotation>()
    {
        new Annotation(){
            ElementRevitId = "26921472",
                AnnotationId = AnnotationObject.RandomString(5),
                Assignee = "joe1",
                Message = "TestAnnotation1"
        },
        new Annotation(){
            ElementRevitId = "26224929",
                AnnotationId = AnnotationObject.RandomString(5),
                Assignee ="Marie2",
                Message = "TestAnnotation2",
        },
        new Annotation(){
            ElementRevitId = "2115662",
                AnnotationId = AnnotationObject.RandomString(5),
                Assignee = "George3",
                Message = "TestAnnotation3"
        }
    };

}


