using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextShape : MonoBehaviour
{
    [SerializeField] private Shape _shapeDisplay;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.ShapeSpawner.OnShapeSpawned.AddListener(DisplayNextShape);
    }

    private void DisplayNextShape(string shapeString)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        _shapeDisplay.SpawnSprites(shapeString, out Vector3 pivotOffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
