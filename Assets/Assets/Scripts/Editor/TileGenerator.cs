using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class TileGenerator : EditorWindow
{
    //Referencing code from Yoav's flower gear generator from 2nd year
    private GameObject parent, childPrefab;
    private int rows=1, columns=1;

    [MenuItem("Custom/Tile Generator")]
    public static void ShowWindow()
    {
        GetWindow<TileGenerator>("Tile Generator");
    }

    private void OnEnable()
    {
        if (Selection.activeGameObject)
        {
            parent = Selection.activeGameObject;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Generator Settings", EditorStyles.boldLabel);
        
        parent = EditorGUILayout.ObjectField("Parent Object", parent, typeof(GameObject), true) as GameObject;
        childPrefab = EditorGUILayout.ObjectField("Parent Object", childPrefab, typeof(GameObject), true) as GameObject;
        rows = EditorGUILayout.IntField("Number Of Rows", rows);
        columns = EditorGUILayout.IntField("Number Of Columns", columns);
        
        GUILayout.Space(10);

        if (GUILayout.Button("Instantiate Prefabs"))
        {
            Debug.Log("The button got pressed woo");
            if (parent & rows > 0 && columns > 0)
            {
                InstantiateGrid();
            }
            else
            {
                Debug.LogError("Please select a gameObject as the parent and ensure rows and columns are a positive nonzero number");
            }
        }

        if (GUILayout.Button("Clear Children"))
        {
            ClearChildren();
        }
    }

    private void InstantiateGrid()
    {
        //this is a grid of hexagons, top and bottom are flat
        float incSizeV = 0.5f*Mathf.Sqrt(3), incSizeH = 0.5f;
        float incrementSizeVert = incSizeV * parent.transform.lossyScale.y;
        float incrementSizeHoriz = incSizeH * parent.transform.lossyScale.x;
        Vector2 parentPos = parent.transform.position;
        Vector2 placementPos = parentPos;
        for (int i = 0; i < rows; i++)
        {
            if (i % 2 == 1)
            {
                placementPos.y = incrementSizeVert / -2f;
            }
            else
            {
                placementPos.y = 0;
            }
            for (int j = 0; j < columns; j++)
            {
                GameObject tile = (GameObject) PrefabUtility.InstantiatePrefab(childPrefab, parent.transform);
                tile.transform.position = placementPos;
                placementPos.y -= incrementSizeVert;
            }
            placementPos.x += incrementSizeHoriz + incrementSizeHoriz/2f;
        }
    }

    private void ClearChildren()
    {
        //foreach transform(Which must exist)
        foreach (var child in parent.GetComponentsInChildren<Transform>())
        {
            if (child != parent.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
