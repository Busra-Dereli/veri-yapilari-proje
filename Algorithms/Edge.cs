using System.Collections.Generic;
using System;
 
    public class Edge
{
    public int Source {get ; set;}
    public int Destination {get ; set;}
    public double Weight{get; set;}

    public Edge(int source,int destination,double weight)
    {
        Source=source;
        Destination=destination;
        Weight=weight;
    }
    public Edge(int source,int destination) : this(source,destination,1.0)
    {
        
    }
}