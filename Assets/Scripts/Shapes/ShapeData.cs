using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ShapeData : ISerializationCallbackReceiver
{
    public string ShapeName;
    public bool Enabled;
    [Tooltip("Determines the likeliness of a piece being selected")] public float DropWeighting;
    public string ShapeString;

    public void OnAfterDeserialize()
    {
        if (Enabled && DropWeighting < 1)
        {
            DropWeighting = 1;
        }

        if (!Enabled)
        {
            DropWeighting = 0;
        }
    }

    public void OnBeforeSerialize()
    {
    }
}
