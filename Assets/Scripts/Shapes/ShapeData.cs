using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ShapeData : ISerializationCallbackReceiver
{
    public string ShapeName;
    public bool Enabled;
    [Tooltip("This can be any number!!!")] public float DropChance;

    public void OnAfterDeserialize()
    {
        if (Enabled && DropChance < 1)
        {
            DropChance = 1;
        }

        if (!Enabled)
        {
            DropChance = 0;
        }
    }

    public void OnBeforeSerialize()
    {
    }
}
