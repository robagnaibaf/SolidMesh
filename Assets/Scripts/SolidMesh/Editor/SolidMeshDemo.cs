using UnityEngine;
using System.Collections;
using UnityEditor;
using log4net.Util;

[CustomEditor(typeof(SolidMeshBehaviour))]
public class SolidMeshBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        SolidMeshBehaviour t = (SolidMeshBehaviour)target;
     
        EditorGUI.BeginChangeCheck(); 
        t.mesh = (Mesh)EditorGUILayout.ObjectField("Input Mesh", t.mesh, typeof(Mesh), true);
        t.swapYZ = EditorGUILayout.Toggle("Swap YZ", t.swapYZ);
        t.transformToUnit = EditorGUILayout.Toggle("Transform to unit", t.transformToUnit);
        t.moveToGround = EditorGUILayout.Toggle("Move to ground", t.moveToGround);
        t.flipNormals = EditorGUILayout.Toggle("Flip Normals", t.flipNormals);
        if (EditorGUI.EndChangeCheck())
            t.Reimport();

        if (GUILayout.Button("Reimport SolidMesh"))
            t.Reimport();

        if (t.IsCreated)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Laplacian Smoothing"))
            {
                t.Smooth();
            }
            if (GUILayout.Button("Loop Subdivision"))
            {
                t.Subdivide();
            }

            EditorGUILayout.EndVertical();
        }
    }
}

