using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IndexMatrix
{
    // Linearized list of elements of an N-by-3 integer matrix
    private List<int> ind;
    
    // Number of rows
    public int Count
    {
        get => ind.Count / 3;
    }



    // Create from index list
    public IndexMatrix(List<int> indices)
    {
        if (indices.Count % 3 != 0)
            Debug.LogError("IndexMatrix.cs: failed to create indexmatrix, the number of indices must be a multiple of 3.");

        ind = indices;
    }

    // Create and fill with a given value
    public IndexMatrix(int size, int defaultValue)
    {
        ind = new List<int>(3 * size);

        for (int i = 0; i < 3 * size; i++)
        {
            ind.Add(defaultValue);
        }
    }

    // Indexer
    public int this[int i, int j]
    {
        get => ind[3 * i + (j % 3)];
        set => ind[3 * i + (j % 3)] = value;
    }

    // Set an (i,j,k) triple to a given index
    public void Set(int index, int i, int j, int k)
    {
        ind[3 * index] = i;
        ind[3 * index + 1] = j;
        ind[3 * index + 2] = k;
    }

    // Add a new (i,j,k) triple
    public void Add(int i, int j, int k)
    {
        ind.AddRange(new int[] { i, j, k });
    }

    // Returns indices as an array
    public int[] ToArray()
    {
        return ind.ToArray();
    }

    // Converts IndexMatrix to string
    public override string ToString()
    {
        int size = ind.Count * (1 + Mathf.CeilToInt(ind.Count));
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < Count; i++)
        {
            s.Append(string.Format("{0} {1} {2} \n", ind[3 * i], ind[3 * i + 1], ind[3 * i + 2]));
        }
        return s.ToString();
    }
    

    // Get row indices, where the row contains
    // at least one negative entry.
    public List<int> GetInvalidRows()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < Count; i++)
            if (ind[3 * i] < 0 || ind[3 * i + 1] < 0 || ind[3 * i + 2] < 0)
                indices.Add(i);

        return indices;
    }

}

