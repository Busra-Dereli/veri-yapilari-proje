using System.Collections.Generic;

public class PrimAlgorithm
{
    public List<Edge> Run(int startNode, Dictionary<int, List<Edge>> adjacencyList)
    {
        List<Edge> mstEdges =new List<Edge>();

        HashSet<int> visited = new HashSet<int>();

        MinHeap minHeap = new MinHeap();
        visited.Add(startNode);
        foreach(var kenar in adjacencyList[startNode])
        {
            minHeap.Insert(kenar);
        }
        while (visited.Count < adjacencyList.Count)
        {
            Edge currentEdge=minHeap.ExtractMin();
            if(currentEdge==null) break;
            if (visited.Contains(currentEdge.Destination))
            {
                continue;
            }
            mstEdges.Add(currentEdge);
            visited.Add(currentEdge.Destination);
            foreach(var kenarlar in adjacencyList[currentEdge.Destination])
            {
                if (!visited.Contains(kenarlar.Desination))
                {
                    minHeap.Insert(kenarlar);
                }
            }
        }
        return mstEdges;
    }
}
