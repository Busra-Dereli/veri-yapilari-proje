using System;

namespace PCBBaglantiAgiOptimizasyonu
{
    public class Edge 
    {
        public Node Destination; 
        public int Weight;       
        public Edge Next;

        public Edge(Node destination, int weight = 1) 
        {
            this.Destination = destination;
            this.Weight = weight;
            this.Next = null;
        }
    }
}