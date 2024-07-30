using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System;
using System.Linq;

public class MeshImporter
{
    [Flags]
    public enum Transformation
    {
        None = 0,
        TransformToUnit = 1,
        SwapYZ = 2,
        MoveToGround = 4,
        FlipNormals = 8
    }


    private Vector3[] vertices;
    private int[] indices;


    public Vector3[] GetVertices() { return vertices; }
    public int[] GetIndices() { return indices; }

    public void LoadFromFile(string path)
    {
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        if (!File.Exists(path))
        {
            Debug.Log("File doesn't exists: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string[] separator = new string[] { " " };
            string[] values = lines[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            if (values.Length == 4)
            {
                if (values[0] == "v")
                {
                    float x = float.Parse(values[1], NumberStyles.Any, ci);
                    float y = float.Parse(values[2], NumberStyles.Any, ci);
                    float z = float.Parse(values[3], NumberStyles.Any, ci);
                    vertices.Add(new Vector3(x, y, z));
                }
                if (values[0] == "f")
                {
                    int i1 = int.Parse(values[1]) - 1;
                    int i2 = int.Parse(values[2]) - 1;
                    int i3 = int.Parse(values[3]) - 1;
                    indices.Add(i1);
                    indices.Add(i2);
                    indices.Add(i3);
                }
            }
        }
        this.vertices = vertices.ToArray();
        this.indices = indices.ToArray();
    }

    public void LoadFromMesh(Mesh mesh)
    {
        vertices = mesh.vertices.Clone() as Vector3[];
        indices = mesh.GetIndices(0) as int[];
    }

    public void TransformMesh(Transformation transformation)
    {
        if ((transformation & Transformation.SwapYZ) != 0)
            SwapYZ();
        if((transformation & Transformation.TransformToUnit) != 0)
            TransformToUnit();
        if ((transformation & Transformation.MoveToGround) != 0)
            MoveToGround();
        if ((transformation & Transformation.FlipNormals) != 0)
            FlipNormals();
    }

    public void SwapYZ()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices[i].x,
                                      vertices[i].z,
                                      vertices[i].y);
        }
    }

    public void TransformToUnit()
    {
        float maxDist = 0f;
        Vector3 centroid = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < vertices.Length; i++)
            centroid += vertices[i];
        centroid /= vertices.Length;
        for (int i = 0; i < vertices.Length; i++)
        {
            float dist = Vector3.SqrMagnitude(centroid - vertices[i]);
            if (dist > maxDist)
                maxDist = dist;
        }
        maxDist = Mathf.Sqrt(maxDist);
        for(int i=0; i< vertices.Length; i++)
        {
            vertices[i] = (vertices[i] - centroid) / maxDist;
        }
    }

    private void MoveToGround()
    {
        float miny = float.PositiveInfinity;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y < miny)
                miny = vertices[i].y;
        }

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] -= new Vector3(0f, miny, 0f); 
    }

    public void FlipNormals()
    {
        int j = 0;
        for(int i=0; i<indices.Length / 3; i++)
        {
            // Swap 1st and 3rd index in each triangle 
            // (p,q,r) -> (r,q,p)
            j = indices[3 * i];
            indices[3 * i] = indices[3 * i + 2];
            indices[3 * i + 2] = j;
        }
    }


    private float quantize(float x, float epsilon)
    {
        return Mathf.Round(x / epsilon) * epsilon;
    }
    private Vector3 quantize(Vector3 v, float epsilon)
    {
        return new Vector3(quantize(v.x, epsilon),
                           quantize(v.y, epsilon),
                           quantize(v.z, epsilon));
    }

    public void Quantize(float epsilon)
    {
        for(int i=0; i<vertices.Length; i++)
        {
            vertices[i] = quantize(vertices[i], epsilon);
        }
    }
}
