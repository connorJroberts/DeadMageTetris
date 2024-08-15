using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ShapeData))]
public class ShapeUIE : PropertyDrawer
{

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();
        
        PropertyField nameAttribute = new PropertyField(property.FindPropertyRelative("ShapeName"));
        PropertyField enabledAttribute = new PropertyField(property.FindPropertyRelative("Enabled"));
        PropertyField chanceAttribute = new PropertyField(property.FindPropertyRelative("DropWeighting"));

        container.Add(nameAttribute);
        container.Add(enabledAttribute);
        container.Add(chanceAttribute);

        return container;
    }

    }
