// Proje Ekibi:
// Busra Dereli

namespace PCBBaglantiAgiOptimizasyonu
{
    public class PrimAlgorithm
    {
        public Edge[] Run(Node startNode, Graph pcbGraph)
        {
            Edge[] mstEdges = new Edge[pcbGraph.ComponentCount];
            int mstCount = 0;
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
                if (currentEdge == null) break;

                if (currentEdge.Destination.IsVisited)
                {
                    continue;
                }

                mstEdges[mstCount++] = currentEdge;
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

            // Sadece dolu olan kismi don
            Edge[] result = new Edge[mstCount];
            for (int i = 0; i < mstCount; i++)
                result[i] = mstEdges[i];

            return result;
        }
    }
}
