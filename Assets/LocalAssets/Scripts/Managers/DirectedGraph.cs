using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DirectedGraph<Data> where Data : class
{
    List<Node> _nodes = new List<Node>();

    public class Node
    {
        //node constructor
        Data _data;
        List<Node> _incoming = new List<Node>();
        List<Node> _outgoing = new List<Node>();

        public Node(Data data)
        {
            _data = data;
        }

        public Data getData()
        {
            return _data;
        }

        public List<Node> getIncoming()
        {
            return _incoming;
        }

        public List<Node> getOutgoing()
        {
            return _outgoing;
        }
    }


    public Node addNode(Data data)
    {
        Node node = new Node(data);
        _nodes.Add(node);
        return node;
    }

    public Node findNode(Data data)
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].getData() == data)
            {
                return _nodes[i];
            }
        }
        return null;
    }

    public void addEdge(Node sourceNode, Node destinationNode)
    {
        if (sourceNode == null)
        {
            Debug.LogWarning("No source Node");
        }
        else if (destinationNode == null)
        {
            Debug.LogWarning("No destination Node");
        }
        else if (sourceNode == destinationNode)
        {
            Debug.LogWarning("Source node and destination node are the same");
        }

        sourceNode.getOutgoing().Add(destinationNode);
        destinationNode.getIncoming().Add(sourceNode);
    }

    public void addEdge(Data source, Data destination)
    {
        addEdge(findNode(source), findNode(destination));
    }

}
