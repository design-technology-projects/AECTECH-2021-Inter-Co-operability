using Speckle.ConnectorUnity;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static AnnotationsHandler;

public class SpeckleAnnotationManager : MonoBehaviour
{
    // Speckle connection reference
    StreamManager manager;
    Sender sender;
    RecursiveConverter recursiveConvertor;

    // Reflect annotation reference
    private AnnotationsManager annotationManager;

    [SerializeField]
    private string streamId;

    private GameObject annotationMarkups;
    public List<Annotation> streamAnnotations;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<StreamManager>();
        sender = gameObject.AddComponent<Sender>();
        recursiveConvertor = gameObject.AddComponent<RecursiveConverter>();
    }

    // Update is called once per frame
    void Update()
    {
        // Assign a strem manager if exists and display selected stream id (just for reference)
        if (annotationManager == null && AnnotationsManager.instance != null) annotationManager = AnnotationsManager.instance;
        if (manager.SelectedStream != null)  streamId = manager.SelectedStream.id;
    }

    public void SendAnnotation()
    {
        // Clean up lists, they will be now updated with the annotations from annotations object storage
        if (annotationMarkups != null) Destroy(annotationMarkups);
        if (streamAnnotations != null) streamAnnotations.Clear();

        List<Annotation> annotationsToSend = annotationManager.AnnotationsObjectStorage.Select(x => (Annotation)x).ToList();

        // Send to Speckle! We are creating a Speckle class Base and assigning our annotations under the key "objects"
        var @base = new Base();
        @base["objects"] = annotationsToSend;
        sender.Send(streamId, @base, commitMessage: "Annotations from " + manager.SelectedAccount.userInfo.name + " | Unity");

    }

    public async Task Receive()
    {
        await LoadStreams();

        if (annotationMarkups != null) Destroy(annotationMarkups);
        if (streamAnnotations != null) streamAnnotations.Clear();

        // transport defines the way that Speckle writes to and reads server
        // more info: https://speckle.guide/dev/architecture.html#transports
        var transport = new ServerTransport(manager.SelectedAccount, streamId);
        var commitId = manager.Branches[manager.SelectedBranchIndex].commits.items[manager.SelectedCommitIndex].referencedObject;
        var @base = await Operations.Receive(commitId, remoteTransport: transport);


        // Get objects from stream and cast them into Base class
        var receivedBaseObjects = ((IEnumerable)@base.GetMembers()["objects"]).Cast<Base>().ToList();

        // Map Base class into Annotation class through the json mapping
        streamAnnotations = receivedBaseObjects.Select(x => ConvertBaseToAnnotation((Base)x)).ToList();

        // Get meshes from stream
        annotationMarkups = recursiveConvertor.ConvertRecursivelyToNative(@base, commitId);

        // Extra step: assign namaes of objects with Annotations Id
        if (annotationMarkups != null)
        {
            annotationMarkups.name = String.Format("objects | {0}", manager.SelectedStream.ToString());

            var baseMeshes = annotationMarkups.GetComponentsInChildren<Transform>()
                .Select(x => x.gameObject).Where(x => x.gameObject.name == "Base").ToList();

            for (int i = 0; i < receivedBaseObjects.Count; i++)
            {
                baseMeshes[i].name = String.Format("base | {0}", streamAnnotations[i]["AnnotationId"]);
            }
        }


    }

    private async Task LoadStreams()
    {

        manager.Streams = await manager.Client.StreamsGet();
        manager.Branches = await manager.Client.StreamGetBranches(manager.SelectedStream.id);
        if (manager.Branches.Any())
        {
            manager.SelectedBranchIndex = 0;
            if (manager.Branches[manager.SelectedBranchIndex].commits.items.Any())
            {
                manager.SelectedCommitIndex = 0;
            }
        }
    }

    public Base RecurseTreeToNative(GameObject o)
    {
        return sender.RecurseTreeToNative(o);
    }
    Annotation ConvertBaseToAnnotation(Base b)
    {
        string serializedAnnotation = JsonConvert.SerializeObject(b);
        return JsonConvert.DeserializeObject<Annotation>(serializedAnnotation);
    }
}
