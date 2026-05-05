using System.Collections.Generic;

public class MinHeap
{
    private List<Edge> heapList;
    

    public MinHeap()
    {
     heapList = new List<Edge>();  
    }
    private int GetLeftChildIndex(int index)
    {
        return index*2+1;
    }
    private int GetRightChildIndex(int index)
    {
    return index*2+2;    
    }
    private int GetParentIndex(int index)
    {
        return (index-1)/2;
    }
}