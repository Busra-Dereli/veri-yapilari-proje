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
        return index * 2 + 1;
    }

    private int GetRightChildIndex(int index)
    {
        return index * 2 + 2;    
    }

    private int GetParentIndex(int index)
    {
        return (index - 1) / 2;
    }

    public void Insert(Edge newEdge)
    {
      
        heapList.Add(newEdge);
        
     
        HeapifyUp(heapList.Count - 1);
    }

    private void HeapifyUp(int currentIndex)
    {
        int parentIndex = GetParentIndex(currentIndex);
        
        while(currentIndex > 0 && heapList[currentIndex].Weight < heapList[parentIndex].Weight)
        {
        
            Edge temp = heapList[currentIndex];
            heapList[currentIndex] = heapList[parentIndex];
            heapList[parentIndex] = temp;
            
            
            currentIndex = parentIndex;
            
           
            parentIndex = GetParentIndex(currentIndex);
        }
    }
    public Edge ExtractMin()
    {
        if(heapList.Count==0) {
            return null;
        }
        Edge minEdge=heapList[0];
       heapList[0] = heapList[heapList.Count - 1];
        heapList.RemoveAt(heapList.Count - 1);
         HeapifyDown(0);
         return minEdge;
}
private void HeapifyDown(int currentIndex)
    {
        bool dogruMu =true;
        while (dogruMu)
        {
            int leftChildIndex=GetLeftChildIndex(currentIndex);
            int rightChildIndex=GetRightChildIndex(currentIndex);
            int smallestIndex=currentIndex;
            if(leftChildIndex<heapList.Count && heapList[leftIndexChild].Weight < heapList[smallestIndex].Weight)
            {
                smallestIndex=leftChildIndex;
            }    
            if(rightChildIndex<heapList.Count && heapList[rightChildIndex].Weight < heapList[smallestIndex].Weight)
            {
                smallestIndex=rightChildIndex;
            }
            if (smallestIndex == currentIndex)
            {
                break;
            }
           Edge temp=heapList[currentIndex];
           heapList[currentIndex]=heapList[smallestIndex];
           heapList[smallestIndex]=temp;
           currentIndex=smallestIndex; 



        }
    }
}