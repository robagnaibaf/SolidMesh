using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SolidMesh
{
    public void Smooth()
    {
        List<Vector3> W = new List<Vector3>(VertexCount);

        for(int i=0; i<VertexCount; i++)
            W.Add(Vector3.zero);

        for(int i=0; i<FaceCount; i++)
            for(int j=0; j<3; j++)
                W[T[i, j]] += V[T[i, j + 1]];

        for (int i = 0; i < VertexCount; i++)
            W[i] /= d[i];

        V = W;
    }
}
