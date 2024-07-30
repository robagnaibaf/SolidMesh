using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public partial class SolidMesh
{
    public void Subdivide()
    {
        // Beta function for Loop Subdivision
        Func<int, float> beta = (n => n == 3 ? 3f / 16f : 3f / (8f * (float)n));

        // New vertex positions
        // For each edge we create a new vertex
        List<Vector3> VV = new List<Vector3>(VertexCount + EdgeCount);
        // New vectors for the subdivided mesh
        IndexVector BB = new IndexVector(VertexCount + EdgeCount, -1);
        IndexVector bb = new IndexVector(VertexCount + EdgeCount, -1);
        IndexVector dd = new IndexVector(VertexCount + EdgeCount, 0);

        for (int i = 0; i < VertexCount; i++)
        {
            // Old vertices initialization
            VV.Add((1 - (float)d[i] * beta(d[i])) * V[i]);
        }

        // Edge-vertex matrix
        // E[i,j] = k  <=>  k is the index of the new vertex V[k]
        //                  created on the j-th edge of the triangle T[i]
        IndexMatrix E = new IndexMatrix(FaceCount, -1);

        // Counter for new vertices 
        int k = VertexCount;
        for (int i = 0; i < FaceCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // OLD VERTICES               
                VV[T[i, j]] += (beta(d[T[i, j]])) * V[T[i, j + 1]];
                BB[T[i, j]] = 4 * i + j;
                bb[T[i, j]] = 0;
                dd[T[i, j]] = d[T[i, j]];

                // NEW VERTICES

                // If the new vertex is already created by the
                // adjacent triangle, then copy the new vertex index ... 
                if (E[A[i, j], a[i, j]] >= 0)
                {
                    E[i, j] = E[A[i, j], a[i, j]];
                }
                else
                // ... else create a new one.
                {
                    E[i, j] = k;
                    // Calculating position of the new vertex ...
                    // start vertex of the edge (1/8)
                    VV.Add((3 * V[T[i, j]] +
                            // end vertex of the edge (1/8)
                            3 * V[T[i, j + 1]] +
                            // the 3rd vertex of this triangle (3/8)
                            V[T[i, j + 2]] +
                            // the 3rd vertex of the adjacent triangle (3/8)
                            V[T[A[i, j], a[i, j] + 2]]) / 8);
                    BB[k] = 4 * i + j;
                    bb[k] = 1;
                    // The degree of each new vertex is 6
                    dd[k] = 6;
                    k++;
                }
            }
        }
        // NEW INDICES

        // Subdivision of triangles looks like:
        //
        //          T[i,2]
        //     E[i,2]    E[i,1]
        // T[i,0]   E[i,0]   T[i,1]
        // 
        // where T[i,*] are the old E[i,*] are the
        // newly created vertices.

        IndexMatrix TT = new IndexMatrix(4 * FaceCount, -1);
        IndexMatrix AA = new IndexMatrix(4 * FaceCount, -1);
        IndexMatrix aa = new IndexMatrix(4 * FaceCount, -1);

        for (int i = 0; i < FaceCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                TT.Set(4 * i + j, T[i, j], E[i, j], E[i, j + 2]);
                AA.Set(4 * i + j, 4 * A[i, j] + ((a[i, j] + 1) % 3),
                                  4 * i + 3,
                                  4 * A[i, j + 2] + a[i, j + 2]);
                aa.Set(4 * i + j, 2, j, 0);
            }
            TT.Set(4 * i + 3, E[i, 2], E[i, 0], E[i, 1]);
            AA.Set(4 * i + 3, 4 * i, 4 * i + 1, 4 * i + 2);
            aa.Set(4 * i + 3, 1, 1, 1);
        }
        // return new SolidMesh(VV, TT, AA, aa, BB, bb, dd);
        V = VV; T = TT; A = AA; a = aa; B = BB; b = bb; d = dd;
    }
}
