// #define __DEBUG__SOLIDMESH__

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class SolidMesh
{
    // Vertices
    private List<Vector3> V;

    // Triangles
    private IndexMatrix T;

    // Degree of vertices 
    // d[i] = k <=> V[i] vertex has exactly k vertex neighbours
    private IndexVector d;

    // Adjacency matrix (triangle vs triangle)
    // A[i,j] = k <=>
    // T[i] triangle is adjacent to T[A[i,j]] along the j-th edge
    private IndexMatrix A;

    // Ordering matrix (triangle vs triangle) 
    // a[i,j] = k  <=>
    // T[i] and T[A[i,j]] triangles common edge is the k-th edge of T[A[i,j]]
    private IndexMatrix a;

    // Adjacency vector (vertex vs triangle)
    // B[i] = k  =>
    // V[i] vertex is a vertex of T[k] triangle
    private IndexVector B;

    // Ordering vector (vertex vs triangle)
    // b[i] = k =>
    // V[i] is the k-th vertex of triangle T[B[i]]
    private IndexVector b;


    // Create SolidMesh from vertex and index list
    private const float MIN_TRIANGLE_AREA = 1e-5f;

    public SolidMesh(Vector3[] vertices, int[] indices)
    {
        V = vertices.ToList();
        T = new IndexMatrix(indices.ToList());
        GenerateAdjacencies();

#if __DEBUG__SOLIDMESH__
        Debug.Log(string.Format("New SolidMesh created ({0} vertices, {1} triangles)",
                                                        V.Count, T.Count));
#endif
    }
    public SolidMesh(List<Vector3> vertices, List<int> indices)
    {
        V = vertices;
        T = new IndexMatrix(indices);
        GenerateAdjacencies();

#if __DEBUG__SOLIDMESH__
        Debug.Log(string.Format("New SolidMesh created ({0} vertices, {1} triangles)",
                                                        V.Count, T.Count));
#endif
    }
    public SolidMesh(List<Vector3> V, IndexMatrix T, IndexMatrix A, IndexMatrix a, IndexVector B, IndexVector b, IndexVector d)
    {
        this.V = V; this.T = T; this.A = A; this.a = a; this.B = B; this.b = b; this.d = d;
    }

    // Return the vertices in an array
    public Vector3[] GetVertices()
    {
        return V.ToArray();
    }

    // Return the indices as a linearized array
    public int[] GetIndices()
    {
        //if(!modificationsAllowed)
            return T.ToArray();


        for(int i=0; i<T.Count;i++)
        {

        }
    }
    public Vector3[] GetVerticesOf(int faceIndex)
    {
        return new Vector3[] { V[T[faceIndex, 0]], V[T[faceIndex, 1]], V[T[faceIndex, 2]] };
    }

    public int VertexCount
    {
        get { return V.Count; }
    }
    public int EdgeCount
    {
        get { return T.Count * 3 / 2; }
    }
    public int FaceCount
    {
        get { return T.Count; }
    }
    public int EulerCharacteristic
    {
        get { return V.Count - T.Count / 2; }
    }

    public void GenerateAdjacencies()
    {
        Dictionary<Vector2Int, Vector2Int> N = new Dictionary<Vector2Int, Vector2Int>();
        
        Vector2Int key1, key2, key;
        bool hasKey1, hasKey2;
        Vector2Int val;

        for (int i = 0; i < T.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                key1 = new Vector2Int(T[i, j], T[i, j + 1]);
                key2 = new Vector2Int(T[i, j + 1], T[i, j]);
                val = new Vector2Int(i, j);
                if (!N.ContainsKey(key1))
                {
                    N.Add(key1, val);
                }
                else
                {
                    if (!N.ContainsKey(key2))
                    {
                        N.Add(key2, val);
                    }
                    else
                    {
                        string msg = "ERROR: Degenerate edge found. \n";
                        msg += key1.x + " -> " + key1.y + " edge has more than 2 adjacent triangles.";
                        Debug.LogError(msg);
                    }
                }
            }
        }

        // CHECKPOINT
        // If we do not have error, there are no degenerate edges

        A = new IndexMatrix(T.Count, -1);
        a = new IndexMatrix(T.Count, -1);
        B = new IndexVector(V.Count, -1);
        b = new IndexVector(V.Count, -1);
        d = new IndexVector(V.Count, 0);

        for (int i=0; i<T.Count; i++)
        {
            for (int j=0; j<3; j++)
            {
                key1 = new Vector2Int(T[i, j], T[i, j + 1]);
                key2 = new Vector2Int(T[i, j + 1], T[i, j]);

                hasKey1 = N.ContainsKey(key1);
                hasKey2 = N.ContainsKey(key2);

                if (!hasKey1 || !hasKey2)
                {
                    string msg = "ERROR: Boundary edge found. \n";
                    msg += key1.x + " -> " + key1.y + " edge has only 1 adjacent triangle.";
                    //Debug.LogError(msg);
                    
                    A[i, j] = -1;
                    a[i, j] = -1;

                    key = hasKey1 ? key1 : key2;
                    d[key.x]++;
                    d[key.y]++;                    
                }
                else
                {
                    A[i,j] = N[key1].x == i ? N[key2].x : N[key1].x;
                    a[i,j] = N[key1].x == i ? N[key2].y : N[key1].y;

                    if(key1.x < key1.y)
                    {
                        d[key1.x]++;
                        d[key1.y]++;
                    }
                }

                if (B[T[i,j]] < 0)
                {
                    B[T[i, j]] = i;
                    b[T[i, j]] = j;
                }
            }
        }
#if __DEBUG__SOLIDMESH__
        Debug.Log(A.ToString());
        Debug.Log(a.ToString());
        Debug.Log(B.ToString());
        Debug.Log(b.ToString());
        Debug.Log(d.ToString());
#endif
    }

    public int[] GetFacesOf(int vertexIndex)
    {
        int n = d[vertexIndex];
        int[] v = new int[n]; // vertex indices
        int[] o = new int[n]; // vertices' order within triangles
        int[] f = new int[n]; // triangle indices

        int k = vertexIndex; // query


        f[0] = B[k];
        o[0] = (b[k] + 2) % 3;
        // v[0] = T[f[0], o[0]];
        for(int i=0; i<n - 1; i++)
        {
            f[i+1] = A[f[i], o[i]];
            o[i+1] = (a[f[i], o[i]] + 2) % 3;
            // v[i+1] = T[f[i+1], o[i+1]];
        }
        return f;
    }


    public void Flush()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Triangle; T[i,1]; T[i,2]; T[i,3]; A[i,1]; A[i,2]; A[i,3]; a[i,1]; a[i,2]; a[i,3]");
        for (int i = 0; i < FaceCount; i++)
        {
            sb.AppendLine(string.Format("{0}; {1}; {2}; {3}; {4}; {5}; {6}; {7}; {8}; {9}; ",
                                        i, T[i, 0], T[i, 1], T[i, 2], A[i, 0], A[i, 1], A[i, 2], a[i, 0], a[i, 1], a[i, 2]));
        }
        sb.AppendLine("");
        sb.AppendLine("Vertex; B[i]; b[i]; d[i]; ; ; ; ; ;");
        for (int i = 0; i < VertexCount; i++)
        {
            sb.AppendLine(string.Format("{0}; {1}; {2}; {3}; ; ; ; ; ; ;",
                                        i, B[i], b[i], d[i]));
        }
        sb.AppendLine("");

        Debug.Log(sb.ToString());
    }
}
