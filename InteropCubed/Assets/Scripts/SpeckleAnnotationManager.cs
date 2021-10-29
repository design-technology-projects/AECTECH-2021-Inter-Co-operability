using Speckle.ConnectorUnity;
using Speckle.Core.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Speckle.Core.Kits;
using System;

public class SpeckleAnnotationManager : MonoBehaviour
{

    public bool done = false;
    StreamManager manager;
    public GameObject objectToSend;
    public Base convertedObjectToSend;

    
    System.Random rand;

    string streamID;
    Sender sender;
    Receiver receiver;

    public bool receive = false;
    public bool check = false;
    // Start is called before the first frame update
    void Start()
    {
        // Get stream id
        manager = gameObject.GetComponent<StreamManager>();
        sender = gameObject.AddComponent<Sender>();
        var converter = new Objects.Converter.Unity.ConverterUnity();
        convertedObjectToSend = sender.RecurseTreeToNative(objectToSend);

        receiver = gameObject.AddComponent<Receiver>();
        // stream ID etc
       // streamID = manager.Streams[0].id;
    }

    // Update is called once per frame
    void Update()
    {
        if (!done && manager.Accounts != null && manager.Streams != null)
        {
            streamID = manager.Streams[0].id;
            rand = new System.Random();
            var @base = new Base();

            // list of elements that inherit Speckle.Core.Models.Base
            List<Annotation> data = new List<Annotation>();
            for (int i = 0; i < 10; i++)
            {
                Annotation element = new Annotation
                {
                    ElementRevitId = "Revit element ID",
                    AnnotationId = System.String.Format("{0:0000}", rand.Next(50, 100)),
                    Assignee = "Asignee name",
                    Message = "Annonation message",
                    Mesh = convertedObjectToSend
                };

                data.Add(element);
            }

            @base["objects"] = data;
            sender.Send(streamID, @base);

            done = true;
        }

        if (receive)
        {
            receiver.Init(streamID);
            //receiver.ReceiveAnnotation();
            receiver.Receive();
            var elements = receiver.receivedBase;
            receive = false;
        }

        if (check)
        {
            Debug.Log("check things");
        }
    }
}



public class Annotation : Base
{ 
    public string ElementRevitId { get; set; }
    public string AnnotationId { get; set; }
    public string Assignee { get; set; }
    public string Message { get; set; }
    
    public Base Mesh { get; set; }

}
