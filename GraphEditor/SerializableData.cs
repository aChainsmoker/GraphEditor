using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;

namespace GraphEditor
{
    [Serializable]
    public class SerializableEdge
    {
        public int StartNodeId { get; set; }
        public int EndNodeId { get; set; }
        public bool IsOriented { get; set; }
        public List<Point> InflectionPoints { get; set; } = new List<Point>();
    }

    [Serializable]
    public class SerializableNode
    {
        public int Id { get; set; }
        public string EllipseName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    [Serializable]
    public class SerializableGraph
    {
        public List<SerializableNode> Nodes { get; set; } = new List<SerializableNode>();
        public List<SerializableEdge> Edges { get; set; } = new List<SerializableEdge>();
    }
}
