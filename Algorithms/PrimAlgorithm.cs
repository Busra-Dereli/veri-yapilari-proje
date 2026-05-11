using System.Collections.Generic;

namespace PCBBaglantiAgiOptimizasyonu
{
    public class PrimAlgorithm
    {
      
        public List<Edge> Run(Node startNode, Graph pcbGraph)
        {
            List<Edge> mstEdges = new List<Edge>();
            MinHeap minHeap = new MinHeap();
            
         
            int visitedCount = 0; 
            
            startNode.IsVisited = true;
            visitedCount++;

        
            Edge currentAdj = startNode.HeadEdge;
            while (currentAdj != null)
            {
                minHeap.Insert(currentAdj);
                currentAdj = currentAdj.Next; 
            }

           
            while (visitedCount < pcbGraph.ComponentCount)
            {
                Edge currentEdge = minHeap.ExtractMin();
                if(currentEdge == null) break;

               
                if (currentEdge.Destination.IsVisited)
                {
                    continue;
                }

              
                mstEdges.Add(currentEdge);
                currentEdge.Destination.IsVisited = true;
                visitedCount++;

             
                Edge nextAdj = currentEdge.Destination.HeadEdge;
                while (nextAdj != null)
                {
                   
                    if (!nextAdj.Destination.IsVisited)
                    {
                        minHeap.Insert(nextAdj);
                    }
                    nextAdj = nextAdj.Next; 
                }
            }
            
            return mstEdges;
        }
    }
}
