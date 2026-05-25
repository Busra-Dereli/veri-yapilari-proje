using System;

namespace PCBBaglantiAgiOptimizasyonu
{
    public class Node 
    {
        public string Id;        
        public Edge HeadEdge;
        public bool IsVisited;
        public Node(string id) 
        {
            this.Id = id;
            this.HeadEdge = null;
            this.IsVisited = false;
        }

        public void AddEdge(Node destination, int weight = 1) 
        {
            Edge newEdge = new Edge(destination, weight);
            
            if (HeadEdge == null) 
            {
                HeadEdge = newEdge;
            } 
            else 
            {
                Edge current = HeadEdge;
                while (current.Next != null) 
                {
                    current = current.Next;
                }
                current.Next = newEdge;
            }
        }
    }
}