using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IndexVector
{
    private List<int> ind;

    // Number of elements
    public int Count
    {
        get => ind.Count;
    }

    // Create from index list
    public IndexVector(List<int> indices)
    {
        ind = indices;
    }

    // Create and fill with a given value
    public IndexVector(int size, int defaultValue)
    {
        ind = new List<int>(size);
        for (int i = 0; i < size; i++)
        {
            ind.Add(defaultValue);
        }
    }

    // Indexer
    public int this[int i]
    {
        get => ind[i];
        set => ind[i] = value;
    }

    // Returns indices as an array
    public int[] ToArray()
    {
        return ind.ToArray();
    }

    // Converts IndexVector to string
    public override string ToString()
    {
        int size = ind.Count * (1 + Mathf.CeilToInt(ind.Count));
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < Count; i++)
        {
            s.Append(ind[i] +"\n");
        }
        return s.ToString();
    }

    // Returns the index of the first element,
    // which is greater than the given index.
    // If there is no element greater than the given,
    // it returns -1.
    public int GetFirstGreaterThan(int index)
    {
        for(int i=0; i<Count; i++)
        {
            if (ind[i] > index)
                return i;
        }
        return -1;
    }
}

