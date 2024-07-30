using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SolidMeshBehaviour : MonoBehaviour
{
    public Mesh mesh;

    public bool swapYZ, transformToUnit, moveToGround, flipNormals;
    private MeshImporter.Transformation transformation;

    MeshImporter meshImporter;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    SolidMesh solidMesh;

    private bool isCreated = false;
    private float time;

    public bool IsCreated
    {
        get { return isCreated; }
    }
    public void Initialize()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshImporter = new MeshImporter();
    }

    public void Reimport()
    {
        Import();
        Transform();
        Create();
        Redraw();
    }

    public void Import()
    {
        if (meshImporter == null)
            Initialize();

        meshImporter.LoadFromMesh(mesh);
    }

    public void Transform()
    {
        transformation = MeshImporter.Transformation.None;
        if (swapYZ)
            transformation |= MeshImporter.Transformation.SwapYZ;
        if (transformToUnit)
            transformation |= MeshImporter.Transformation.TransformToUnit;
        if (moveToGround)
            transformation |= MeshImporter.Transformation.MoveToGround;
        if (flipNormals)
            transformation |= MeshImporter.Transformation.FlipNormals;


        if (meshImporter == null)
            Initialize();
        
        meshImporter.TransformMesh(transformation);
    }

    public void Create()
    {
        tic();
        solidMesh = new SolidMesh(meshImporter.GetVertices(), meshImporter.GetIndices());
        Debug.Log(string.Format("A new SolidMesh is created ({0} ms)", toc()));
        isCreated = true;
    }

    public void Redraw()
    {
        tic();        
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.vertices = solidMesh.GetVertices();
        meshFilter.sharedMesh.SetIndices(solidMesh.GetIndices(), MeshTopology.Triangles, 0);
        
        meshFilter.sharedMesh.RecalculateBounds();
        meshFilter.sharedMesh.RecalculateNormals();

        Debug.Log(string.Format("Redrawing SolidMesh after manipulation ({0} ms)", toc()));
    }


    public void Smooth()
    {
        tic();
        solidMesh.Smooth();
        Debug.Log(string.Format("Laplacian Smoothing on SolidMesh ({0} ms)", toc()));

        Redraw();
    }

    public void Subdivide()
    {
        if (solidMesh.VertexCount + solidMesh.EdgeCount > 65536)
        {
            Debug.LogWarning("The resulting mesh would have too many vertices for rendering with 16-bit index buffer. Operation is ignored.");
            return;
        }

        int initialVertexCount = solidMesh.VertexCount;
        
        tic();
        solidMesh.Subdivide();
        Debug.Log(string.Format("Loop Subdivision on SolidMesh ({0} -> {1} vertices, {2} ms)", initialVertexCount, solidMesh.VertexCount, toc()));

        Redraw();
    }

    private void tic()
    {
        time = Time.realtimeSinceStartup;
    }

    private float toc()
    {
        return 1000f * (Time.realtimeSinceStartup - time);
    }
}
