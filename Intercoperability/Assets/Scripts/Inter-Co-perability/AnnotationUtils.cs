using Objects.Converter.Unity;
using Speckle.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AnnotationUtils
{
    public static Base RecurseTreeToNative(GameObject go)
    {
        var converter = new ConverterUnity();
        if (converter.CanConvertToSpeckle(go))
        {
            try
            {
                return converter.ConvertToSpeckle(go);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        if (go.transform.childCount > 0)
        {
            var @base = new Base();
            var objects = new List<Base>();
            for (var i = 0; i < go.transform.childCount; i++)
            {
                var goo = RecurseTreeToNative(go.transform.GetChild(i).gameObject);
                if (goo != null)
                    objects.Add(goo);
            }

            if (objects.Any())
            {
                @base["objects"] = objects;
                return @base;
            }
        }

        return null;
    }
}
