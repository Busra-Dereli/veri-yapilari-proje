using System;

namespace PCBBaglantiAgiOptimizasyonu
{
    // Proje Ekibi:
    // Busra Dereli
    public class Edge 
    {
        public Node Source;      
        public Node Destination; 
        public int Weight;       
        public Edge Next;

        public Edge(Node source, Node destination, int weight = 1) 
        {
            this.Source = source;
            this.Destination = destination;
            this.Weight = weight;
            this.Next = null;
        }
    }
}
